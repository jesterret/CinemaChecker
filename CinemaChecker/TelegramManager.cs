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
using Newtonsoft.Json;

namespace CinemaChecker
{
    class TelegramManager
    {
        TelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("Telegram_ApiToken.txt"));
        Dictionary<string, ChatPreferences> ChatSettings;
        CinemaChecker checker = new CinemaChecker();
        internal Task<User> MeResult = null;

        public TelegramManager()
        {
            var DeserializedTest = ReadSettings();
            if (DeserializedTest != null)
                ChatSettings = DeserializedTest;
            else
                ChatSettings = new Dictionary<string, ChatPreferences>();
            
            bot.OnMessage += Bot_OnMessage;
            bot.OnInlineQuery += Bot_OnInlineQuery;
            bot.OnCallbackQuery += Bot_OnCallbackQuery;

            bot.OnReceiveError += Bot_OnReceiveError;
            bot.OnReceiveGeneralError += Bot_OnReceiveGeneralError;

            MeResult = bot.GetMeAsync();
            bot.StartReceiving(new[]
                {
                    UpdateType.CallbackQueryUpdate,
                    UpdateType.InlineQueryUpdate,
                    UpdateType.MessageUpdate
                });
        }
        ~TelegramManager()
        {
            SaveSettings();
            bot.StopReceiving();
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
                var Params = callback.Data.Split('|');
                if (Params[0] == "Nay")
                { 
                    bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, string.Format("Tough luck {0}!",
                        string.IsNullOrEmpty(callback.From.Username) ? callback.From.FirstName : callback.From.Username),
                        replyMarkup: new InlineKeyboardMarkup(new [] { new InlineKeyboardButton("Yay", Params[1]) }));
                }
                else if(Params.Length == 2)
                {
                    var settings = GetChatSettings(callback.From.Id);
                    if (settings != null && settings.Sites.Count > 0)
                    {
                        var code = Params[0];
                        var date = Params[1];
                        var sites = checker.GetFeatured(code);
                        foreach (var trackedsite in settings.Sites)
                        {
                            var site = sites.SingleOrDefault(s => s.Id == trackedsite);
                            var sitetimes = site?.Features
                                .SingleOrDefault(feat => feat.Date == date)
                                ?.Presentations
                                .Partition(2);
                            var times = sitetimes
                                .Select(pres =>
                                {
                                    return pres.Select(
                                        x => new InlineKeyboardButton(string.Format("{0},{1}", x.Time, x.Tags), "")
                                            {
                                                Url = x.GetUrl(site.Id)
                                            }
                                        ).ToArray();
                                }).ToList();
                            times.Add(new[] { new InlineKeyboardButton("Go Back", Params[0]) });
                            bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, "Select the time:", replyMarkup:
                                new InlineKeyboardMarkup(times.ToArray()));
                        }
                    }
                }    
                else // callback.Data == movie code
                {
                    var settings = GetChatSettings(callback.From.Id);
                    if (settings != null && settings.Sites.Count > 0)
                    {
                        var sites = checker.GetFeatured(callback.Data);
                        List<string> DatesMessage = new List<string>();
                        foreach (var trackedsite in settings.Sites)
                        {
                            var site = sites.SingleOrDefault(s => s.Id == trackedsite);
                            site?.Features.ForEach(feat => DatesMessage.Add(feat.Date));
                        }
                        if (DatesMessage.Count > 0)
                        {
                            var buttons = DatesMessage
                                .Partition(2)
                                .Select(date =>
                                    {
                                        return date.Select(d => new InlineKeyboardButton(d, string.Format("{0}|{1}", Params[0], d))).ToArray();
                                    }).ToList();
                            buttons.Add(new[] { new InlineKeyboardButton("Refresh", callback.Data) });
                            bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, "Select the date:", replyMarkup:
                                new InlineKeyboardMarkup(buttons.ToArray()));
                        }
                        else
                            bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, "Your prefered cinema site does not sell tickets for this movie.");
                    }
                    else
                        bot.EditInlineMessageCaptionAsync(callback.InlineMessageId, $"{callback.From.Username} have not defined their prefered cinema site",
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
                bot.AnswerCallbackQueryAsync(callback.Id);
            }
            else if (callback.Message != null)
            {
                var msg = callback.Message;
                switch(callback.Data)
                {
                    case "bot_showsettings":
                        bot.EditMessageTextAsync(msg.Chat.Id, msg.MessageId, "You can view your current chat settings here.",
                                replyMarkup: new InlineKeyboardMarkup(Bot_SettingsInlineKeyboard));
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
                            ChatSettings[msg.Chat.Id].Remove(sID);
                            ShowTrackedSites(msg.Chat.Id, msg.MessageId);
                        }
                        break;
                }
                bot.AnswerCallbackQueryAsync(callback.Id);
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
            bot.SendTextMessageAsync(PrivateChatID, string.Format("{0}: {1}", msg.From.Username, msg.Text), disableNotification: true).Wait();

            // bot commands handling
            for (int i = 0; i < msg.Entities.Count; i++)
            {
                var botcommand = msg.EntityValues[i];
                if (msg.Entities[i].Type == MessageEntityType.BotCommand)
                {

                    // Verify whether the command is sent on chat with multiple bots
                    // and is intended for our bot
                    var command = botcommand.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                    if (command.Length == 2 && command[1] != BotUsername)
                        return;

                    switch (command[0])
                    {
                        case "/start":
                        case "/registercinema":
                            AddTrackedChat(msg.Chat.Id);

                            var cinemas = checker.GetSites();
                            var sites = cinemas
                                .OrderByDescending(site => site.Name.Contains("Kraków")) // Hax to show closest sites first
                                .ThenBy(site => site.Name);

                            var buttons = sites.Select(site => new[] { new KeyboardButton(site.Name) });
                            bot.SendTextMessageAsync(msg.Chat.Id, Bot_SelectPreferedSite,
                                replyToMessageId: msg.MessageId,
                                replyMarkup: new ReplyKeyboardMarkup
                                {
                                    Keyboard = buttons.ToArray(),
                                    Selective = true
                                });
                            break;
                        case "/registermovie":
                            if (ChatSettings.ContainsKey(msg.Chat.Id))
                                bot.SendTextMessageAsync(msg.Chat.Id, Bot_TypeMovieRegex,
                                    replyToMessageId: msg.MessageId,
                                    replyMarkup: new ForceReply() { Force = true, Selective = true });
                            else
                                bot.SendTextMessageAsync(msg.Chat.Id, "You have not registered any cinemas for this chat.");
                            break;

                        case "/settings":
                            bot.SendTextMessageAsync(msg.Chat.Id, "You can view your current chat settings here.",
                                replyMarkup: new InlineKeyboardMarkup()
                                {
                                    InlineKeyboard = Bot_SettingsInlineKeyboard
                                });

                            break;
                        case "/devbreakquitnowit'snotreallythatone":
                            bot.SendTextMessageAsync(PrivateChatID, "Quitting").Wait();
                            SaveSettings();
                            Environment.Exit(-1);
                            break;
                        default:
                            SaveSettings();
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
                    if (parent.Text == Bot_SelectPreferedSite)
                        TrySite(ref msg);
                    else
                        TryTitle(ref msg);
                }
                else if(msg.Chat.Type == ChatType.Private)
                    TryTitle(ref msg);
            }    
            else if (msg.Chat.Type == ChatType.Private && msg.Entities.Count == 0)
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
                                new InlineKeyboardButton("Nay", string.Format("Nay|{0}", movie.Code))
                            }
                        }
                    }
                });
                bot.AnswerInlineQueryAsync(e.InlineQuery.Id, query.Take(50).ToArray());
            }
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            bot.SendTextMessageAsync(PrivateChatID, $"Error: {e.ApiRequestException.Message}\n");
            bShouldIgnore = true;
        }
        private void Bot_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            bot.SendTextMessageAsync(PrivateChatID, $"Error: {e.Exception.InnerException?.Message}\n");
            bShouldIgnore = true;
        }
        
        private void TrySite(ref Message msg)
        {
            if (TryTrackSite(msg.Text, out Site found))
            {
                AddTrackedChat(msg.Chat.Id, found.Id);
                bot.SendTextMessageAsync(msg.Chat.Id, "Selected and saved: " + msg.Text,
                    replyToMessageId: msg.ReplyToMessage != null ? msg.ReplyToMessage.MessageId : 0,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
        private void TryTitle(ref Message msg)
        {
            if (!string.IsNullOrEmpty(msg.Text))
            {
                AddTrackedTitle(msg.Chat.Id, new Regex(msg.Text));
                bot.SendTextMessageAsync(msg.Chat.Id, "I'll look for titles matching this regexp from now on: " + msg.Text,
                    replyToMessageId: msg.ReplyToMessage != null ? msg.ReplyToMessage.MessageId : 0,
                    replyMarkup: new ReplyKeyboardRemove());
            }
        }
        private bool TryTrackSite(string Name, out Site foundsite)
        {
            foundsite = checker.GetSites()
                .SingleOrDefault(site => site.Name == Name);
            return foundsite != null;
        }

        private ChatPreferences GetInitChatSettings(string ChatID)
        {
            if (!ChatSettings.ContainsKey(ChatID))
                ChatSettings.Add(ChatID, new ChatPreferences());

            return GetChatSettings(ChatID);
        }
        private ChatPreferences GetChatSettings(string ChatID)
        {
            if (ChatSettings.TryGetValue(ChatID, out var Value))
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

            Check(ChatID);
        }
        private void ShowTrackedSites(ChatId ChatID, int MessageID)
        {
            List<InlineKeyboardButton[]> TrackedNames = new List<InlineKeyboardButton[]>();
            var settings = GetChatSettings(ChatID);
            if (settings != null)
            {
                foreach (var s in checker.GetSites())
                {
                    if(settings.Contains(s.Id))
                    {
                        TrackedNames.Add(new[]
                        {
                            new InlineKeyboardButton(s.Name, s.Id.ToString())
                        });
                    }
                }
                //TrackedNames.AddRange(from s in checker.GetSites()
                //                      where settings.Contains(s.Id)
                //                      select new[]
                //                      {
                //                          new InlineKeyboardButton(s.Name, s.Id.ToString())
                //                      });
            }
            TrackedNames.Add(new[] { new InlineKeyboardButton("Go back to settings", "bot_showsettings") });
            bot.EditMessageTextAsync(ChatID, MessageID, "Those are your registered cinema sites. If you'd like to remove one, just click on it.",
                replyMarkup: new InlineKeyboardMarkup(TrackedNames.ToArray()));
        }

        private void SaveSettings()
        {
            if (ChatSettings != null)
            {
                System.IO.File.WriteAllText("Telegram_ChatPreferences.json", JsonConvert.SerializeObject(ChatSettings));
            }
        }
        private Dictionary<string, ChatPreferences> ReadSettings()
        {
            if(System.IO.File.Exists("Telegram_ChatPreferences.json"))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, ChatPreferences>>(System.IO.File.ReadAllText("Telegram_ChatPreferences.json"));
            }

            return null;
        }

        public void Check(ChatId RequestId = null)
        {
            if (RequestId == null)
            {
                foreach (var settings in ChatSettings)
                {
                    var poster = checker.GetPosters()
                        .Where(post => settings.Value.Contains(post.Title)).FirstOrDefault();

                    if (poster != null)
                    {
                    //    var sites = checker.GetFeatured(poster.Code)
                    //        .Where(feat => settings.Value.Contains(feat.Id))
                    //        .Select(feat => feat.Name);

                        bot.SendTextMessageAsync(settings.Key, $"Hey, the movie you were waiting on ('{poster.Title}') is now available!");
                    }
                }
            }
            else
            {
                var settings = GetChatSettings(RequestId);
                var poster = checker.GetPosters()
                    .Where(post => settings.Contains(post.Title)).FirstOrDefault();

                if (poster != null)
                {
                    var sites = checker.GetFeatured(poster.Code)
                        .Where(feat => settings.Contains(feat.Id))
                        .Select(feat => feat.Name);

                    bot.SendTextMessageAsync(RequestId, $"Hey, the movie you were waiting on ('{poster.Title}') is now available!");
                }
            }
        }

        private string BotUsername => MeResult.Result.Username;

        const long PrivateChatID = 0x28d07dc;
        const string Bot_SelectPreferedSite = "Select prefered cinema site";
        const string Bot_TypeMovieRegex = "Type the searched movie name (Regex expected)";
        static bool bShouldIgnore = false;

        private InlineKeyboardButton[][] Bot_SettingsInlineKeyboard = new[]
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
