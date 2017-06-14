using CinemaChecker.CinemaCity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace CinemaChecker
{
    class TelegramManager
    {
        TelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("../../Telegram_ApiToken.txt"));
        Dictionary<string, HashSet<long>> ChatTrackedSites = new Dictionary<string, HashSet<long>>();
        Dictionary<string, HashSet<Regex>> ChatTrackedTitles = new Dictionary<string, HashSet<Regex>>();
        Dictionary<string, ChatPreferences> ChatSettings = new Dictionary<string, ChatPreferences>();
        CinemaChecker checker = new CinemaChecker();
        internal Task<User> MeResult = null;

        public TelegramManager()
        {
            Bot.OnMessage += Bot_OnMessage;
            Bot.OnInlineQuery += Bot_OnInlineQuery;
            Bot.OnCallbackQuery += Bot_OnCallbackQuery;

            Bot.OnReceiveError += Bot_OnReceiveError;
            Bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;

            MeResult = Bot.GetMeAsync();
            Bot.StartReceiving();
        }
        ~TelegramManager()
        {
            Bot.StopReceiving();
        }

        private void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }
            var callback = e.CallbackQuery;
            if (!string.IsNullOrEmpty(callback.InlineMessageId))
            {
                if (callback.Data == "Nay")
                { 
                    Bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, string.Format("Denied by {0}!",
                        string.IsNullOrEmpty(callback.From.Username) ? callback.From.FirstName : callback.From.Username)
                        );
                }
                else
                {
                    if (ChatTrackedSites.ContainsKey(callback.From.Id))
                    {
                        var sites = checker.GetSites();
                        List<string> DatesMessage = new List<string>();
                        foreach (var trackedsite in ChatTrackedSites[callback.From.Id])
                        {
                            var site = sites.Single(s => s.Id == trackedsite);
                            //var presents = site
                            //    .SingleOrDefault(feat => feat.DisplayCode == callback.Data)
                            //    ?.Presentations.Where(pres => pres.IsDubbing == false);

                            //foreach(var pres in presents)
                            //    DatesMessage.Add(pres.ToHtml(trackedsite));
                        }
                        if (DatesMessage.Count > 0)
                        {
                            var str = string.Join(Environment.NewLine, DatesMessage);
                            Bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, "Select the screening time");
                            Bot.SendTextMessageAsync(callback.ChatInstance, str, ParseMode.Html).Wait();
                        }
                        else
                            Bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, "Your prefered cinema site does not sell tickets for this movie.");
                    }
                    else
                        Bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, $"{callback.From.Username} have not defined their prefered cinema site",
                            new InlineKeyboardMarkup
                            {
                                InlineKeyboard = new[]
                                {
                                    new[]
                                    {
                                        new InlineKeyboardButton("Message me now!", "")
                                        {
                                            Url = string.Format("t.me/{0}?start=payload", BotUsername)
                                        }
                                    },
                                    new[]
                                    {
                                        new InlineKeyboardButton("Try again", callback.Data)
                                    }
                                }
                            });
                }
                Bot.AnswerCallbackQueryAsync(callback.Id);
            }
            else if (callback.Message != null)
            {
                var msg = callback.Message;
                switch(callback.Data)
                {
                    case "bot_showsettings":
                        Bot.EditMessageTextAsync(msg.Chat.Id, msg.MessageId, "You can view your current chat settings here.",
                                replyMarkup: new InlineKeyboardMarkup(BOT_SettingsInlineKeyboard));
                        break;
                    case "bot_showsites":
                        ShowTrackedSites(msg.Chat.Id, msg.MessageId);
                        break;
                    case "bot_showprefs":
                        // TODO: Allow to specify and add sorting by chat preferences
                        break;
                    default:
                        long sID = 0;
                        if(long.TryParse(callback.Data, out sID))
                        {
                            ChatTrackedSites[msg.Chat.Id].Remove(sID);
                            ShowTrackedSites(msg.Chat.Id, msg.MessageId);
                        }
                        break;
                }
                Bot.AnswerCallbackQueryAsync(callback.Id);
            }
            return;
        }

        private void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if(bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }
            var msg = e.Message;
            Bot.SendTextMessageAsync(PrivateChatID, string.Format("{0}: {1}", msg.From.Username, msg.Text), disableNotification: true);

            // Bot commands handling
            for (int i = 0; i < msg.Entities.Count; i++)
            {
                var botcommand = msg.EntityValues[i];
                if (msg.Entities[i].Type == MessageEntityType.BotCommand)
                {

                    // Verify whether the command is sent on chat with multiple bots
                    // and is intended for our bot
                    var command = botcommand.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (command.Length == 2 && command[1] == BotUsername)
                        return;

                    switch (command[0])
                    {
                        case "/start":
                        case "/registercinema":
                            AddTrackedChat(msg.Chat.Id);

                            var cinemas = checker.GetSites();
                            var sites = cinemas
                                .OrderByDescending(site => site.Name.Contains("Kraków"))
                                .ThenBy(site => site.Name);

                            var buttons = sites.Select(site => new[] { new KeyboardButton(site.Name) });
                            Bot.SendTextMessageAsync(msg.Chat.Id, BOT_SelectPreferedSite,
                                replyToMessageId: msg.MessageId,
                                replyMarkup: new ReplyKeyboardMarkup
                                {
                                    Keyboard = buttons.ToArray(),
                                    Selective = true
                                });
                            break;
                        case "/registermovie":
                            if (ChatTrackedSites.ContainsKey(msg.Chat.Id))
                                Bot.SendTextMessageAsync(msg.Chat.Id, BOT_TypeMovieRegex,
                                    replyToMessageId: msg.MessageId,
                                    replyMarkup: new ForceReply() { Force = true, Selective = true });
                            else
                                Bot.SendTextMessageAsync(msg.Chat.Id, "You have not registered any cinemas for this chat.");
                            break;

                        case "/settings":
                            Bot.SendTextMessageAsync(msg.Chat.Id, "You can view your current chat settings here.",
                                replyMarkup: new InlineKeyboardMarkup()
                                {
                                    InlineKeyboard = BOT_SettingsInlineKeyboard
                                });

                            break;
                    }
                }
            }

            // Keyboard responses handling
            var parent = msg.ReplyToMessage;
            if (parent != null)
            {
                if (msg.Chat.Type == ChatType.Group) // On group chat, privacy settings on
                {
                    if (parent.Text == BOT_SelectPreferedSite)
                        TrySite(ref msg);
                    else
                        TryTitle(ref msg);
                }
                else if(msg.Chat.Type == ChatType.Private)
                    TryTitle(ref msg);
            }    
            else if (msg.Chat.Type == ChatType.Private)
                TrySite(ref msg);
        }
        private void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }

            var movies = checker.GetPosters()
                .Where(movie => movie.Title.Contains(e.InlineQuery.Query));

            if (movies.Any())
            {
                int ID = 0;
                var query = movies.Select(movie => new InlineQueryResultPhoto()
                {
                    Url = movie.PosterImage,
                    ThumbUrl = movie.PosterImage,
                    Caption = movie.Title,
                    Id = ID++.ToString(),
                    ReplyMarkup = new InlineKeyboardMarkup
                    {
                        InlineKeyboard = new[]
                        {
                            new []
                            {
                                new InlineKeyboardButton("Yay", movie.Code),
                                new InlineKeyboardButton("Nay", "Nay")
                            }
                        }
                    }
                });
                Bot.AnswerInlineQueryAsync(e.InlineQuery.Id, query.Take(50).ToArray());
            }
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            Bot.SendTextMessageAsync(PrivateChatID, $"Error: {e.ApiRequestException.Message}\n");
            bShouldIgnore = true;
        }
        private void Bot_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            Bot.SendTextMessageAsync(PrivateChatID, $"Error: {e.Exception.InnerException?.Message}\n");
            bShouldIgnore = true;
        }
        
        private void TrySite(ref Message msg)
        {
            if (TryTrackSite(msg.Text, out CinemaSite found))
            {
                AddTrackedChat(msg.Chat.Id, found.Id);
                Bot.SendTextMessageAsync(msg.Chat.Id, "Selected and saved: " + msg.Text,
                    replyToMessageId: msg.ReplyToMessage != null ? msg.ReplyToMessage.MessageId : 0,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
        private void TryTitle(ref Message msg)
        {
            if (!string.IsNullOrEmpty(msg.Text))
            {
                AddTrackedTitle(msg.Chat.Id, new Regex(msg.Text));
                Bot.SendTextMessageAsync(msg.Chat.Id, "I'll look for titles matching this regexp from now on: " + msg.Text,
                    replyToMessageId: msg.ReplyToMessage != null ? msg.ReplyToMessage.MessageId : 0,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
        private bool TryTrackSite(string Name, out CinemaSite foundsite)
        {
            foundsite = checker.GetSites()
                .SingleOrDefault(site => site.Name == Name);
            return foundsite != null;
        }

        private ChatPreferences GetInitChatSettings(ChatId ChatID)
        {
            ChatPreferences prefs = new ChatPreferences();
            if (!ChatSettings.ContainsKey(ChatID))
                ChatSettings.Add(ChatID, prefs);

            return prefs;
        }
        private ChatPreferences GetChatSettings(ChatId ChatID)
        {
            if (!ChatSettings.TryGetValue(ChatID, out var Value))
                return Value;

            return null;
        }
        private void AddTrackedChat(ChatId ChatID, long siteId = 0)
        {
            var prefs = GetInitChatSettings(ChatID);

            if (siteId != 0)
                prefs.Add(siteId);
        }
        private void AddTrackedTitle(ChatId ChatID, Regex titleRegex = null)
        {
            var prefs = GetInitChatSettings(ChatID);

            if (titleRegex != null)
                prefs.Add(titleRegex);
        }
        private void ShowTrackedSites(ChatId ChatID, int MessageID)
        {
            List<InlineKeyboardButton[]> TrackedNames = new List<InlineKeyboardButton[]>();
            if (ChatSettings.TryGetValue(ChatID, out ChatPreferences preferences))
            {
                TrackedNames.AddRange(from s in checker.GetSites()
                                      where preferences.Contains(s.Id)
                                      select new[]
                                      {
                                          new InlineKeyboardButton(s.Name, s.Id.ToString())
                                      });
            }
            TrackedNames.Add(new[] { new InlineKeyboardButton("Go back to settings", "bot_showsettings") });
            Bot.EditMessageTextAsync(ChatID, MessageID, "Those are your registered cinema sites. If you'd like to remove one, just click on it.",
                replyMarkup: new InlineKeyboardMarkup(TrackedNames.ToArray()));
        }

        private string BotUsername => MeResult.Result.Username;

        public TelegramBotClient Bot { get => bot; set => bot = value; }

        const long PrivateChatID = 0x28d07dc;
        const string BOT_SelectPreferedSite = "Select prefered cinema site";
        const string BOT_TypeMovieRegex = "Type the searched movie name (Regex expected)";
        static bool bShouldIgnore = false;

        private InlineKeyboardButton[][] BOT_SettingsInlineKeyboard = new[]
        {
            new[]
            {
                new InlineKeyboardButton("Registered sites", "bot_showsites")
            },
            new[]
            {
                new InlineKeyboardButton("Seance preferences", "bot_showprefs")
            }
        };
    }
}
