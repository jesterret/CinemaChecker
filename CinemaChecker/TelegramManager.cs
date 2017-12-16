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
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.InlineKeyboardButtons;

namespace CinemaChecker
{
    class TelegramManager : IDisposable
    {
        TelegramBotClient bot = new TelegramBotClient(System.IO.File.ReadAllText("Telegram_ApiToken.txt"));
        Dictionary<string, ChatPreferences> ChatSettings;
        CinemaChecker checker = new CinemaChecker();
        internal Task<User> MeResult = null;

        public TelegramManager()
        {
            var DeserializedTest = ReadSettings();
            ChatSettings = DeserializedTest ?? new Dictionary<string, ChatPreferences>();
            
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
            Dispose(false);
        }
        
        private async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }
            
            var callback = e.CallbackQuery;
            var data = callback.Data.Split('|');
            var cinema = Cinema.CinemaBase.GetCinema(data[0]);
            if (!string.IsNullOrEmpty(callback.InlineMessageId))
            {
                if(data.Length == 3)
                {
                    InlineKeyboardButton[][] buttons = null;
                    if(data[2] == "movies")
                    {
                        var movies = await cinema.GetRepertoir(data[1]);
                    }
                    else if(data[2] == "upcoming")
                    {
                        var upcoming = await cinema.GetUpcoming(data[1]);
                        buttons = upcoming
                            .Select(up => new InlineKeyboardCallbackButton(up.Title, up.Id))
                            .Partition(2)
                            .ToArray();
                        await bot.EditInlineMessageTextAsync(callback.InlineMessageId, "Here's the list of all upcoming seances");
                    }
                    await bot.EditInlineMessageReplyMarkupAsync(callback.InlineMessageId, new InlineKeyboardMarkup(buttons));
                }
                await bot.AnswerCallbackQueryAsync(callback.InlineMessageId);
            }
            else if(callback.Message != null)
            {
                if (callback.Message.Text == Bot_SelectCinemaSystem)
                {
                    var sites = await Cinema.CinemaBase.GetCinema(data[0]).GetSites();
                    bot.SendTextMessageAsync(callback.From.Id, Bot_SelectSite, replyMarkup: 
                        new InlineKeyboardMarkup(sites
                                        .Select(s => new InlineKeyboardCallbackButton(s.Name, $"{callback.Data}|{s.Id}"))
                                        .Partition(2)
                                        .ToArray()));
                }
                else if(callback.Message.Text == Bot_SelectSite)
                {
                    var upcoming = await Cinema.CinemaBase.GetCinema(data[0]).GetUpcoming(data[1]);
                    bot.SendTextMessageAsync(callback.From.Id, Bot_SelectSite, replyMarkup: 
                        new ReplyKeyboardMarkup(upcoming
                                        .Select(mov => new KeyboardButton(mov.Title))
                                        .Partition(2)
                                        .ToArray()));
                }
            }
            return;
        }
        private async void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            if(bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }

            var msg = e.Message;
            await bot.SendTextMessageAsync(PrivateChatID, $"{msg.From.Username}: {msg.Text}", disableNotification: true);
            
            if (msg.Chat.Type == ChatType.Private)
            {
                // bot commands handling
                for (int i = 0; i < msg.Entities.Count; i++)
                {
                    var botcommand = msg.EntityValues[i];
                    if (msg.Entities[i].Type == MessageEntityType.BotCommand)
                    {
                        // Verify whether the command has appended bot username
                        // and is intended for our bot
                        var command = botcommand.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                        if (command.Length == 2 && command[1] != BotUsername)
                            return;

                        switch (command[0])
                        {
                            case "/start":
                            case "/registercinema":

                                break;

                            case "/registermovie":

                                break;

                            case "/upcoming":
                                bot.SendTextMessageAsync(msg.Chat.Id, Bot_SelectCinemaSystem, replyMarkup: 
                                                        new InlineKeyboardMarkup(Cinema.CinemaBase.CinemaSystems
                                                        .Select(s => new InlineKeyboardCallbackButton(s.Value.GetType().Name, s.Key))
                                                        .Partition(2)
                                                        .ToArray()));
                                break;

                            case "/devbreakquitnow":
                                if (msg.Chat.Id == PrivateChatID)
                                {
                                    bot.SendTextMessageAsync(PrivateChatID, "Quitting").Wait();
                                    SaveSettings();
                                    Program.ShouldQuit.Set();
                                }
                                break;
                        }
                    }
                }

                var parent = msg.ReplyToMessage;
                if(parent != null)
                {

                }
                else if (msg.Entities.Count == 0)
                {
                    var del = await bot.SendTextMessageAsync(msg.Chat.Id, "Clearing reply keyboard", replyMarkup: new ReplyKeyboardRemove());
                    bot.DeleteMessageAsync(msg.Chat.Id, del.MessageId);
                    return;
                }
            } 
        }
        private async void Bot_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            if (bShouldIgnore)
            {
                bShouldIgnore = false;
                return;
            }

            var msg = e.InlineQuery;
            bot.SendTextMessageAsync(PrivateChatID, $"{msg.From.Username}: {msg.Query}", disableNotification: true);

            var data = msg.Query.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if(data.Length >= 1)
            {
                var cinema = Cinema.CinemaBase.GetCinema(data[0]);
                if (cinema != null)
                {
                    if (data.Length == 1)
                    {
                        int i = 0;
                        var sites = await cinema.GetSites();
                        var buttons = sites.Select(s => new InlineQueryResultArticle()
                            {
                                Id = i++.ToString(),
                                Title = s.Name,
                                Description = $"Site ID: {s.Id}",
                                InputMessageContent = new InputTextMessageContent()
                                {
                                    MessageText = $"{s.Name}"
                                },
                                ReplyMarkup = new InlineKeyboardMarkup()
                                {
                                    InlineKeyboard = new[]
                                    {
                                        new[]
                                        {
                                            new InlineKeyboardCallbackButton("Repertoir", $"{data[0]}|{s.Id}|movies"),
                                            new InlineKeyboardCallbackButton("Upcoming", $"{data[0]}|{s.Id}|upcoming")
                                        }
                                    }
                                }
                            });
                        bot.AnswerInlineQueryAsync(msg.Id, buttons.ToArray(), 0, true);
                        // Site list
                    }
                    else if (data.Length == 2)
                    {
                        var sites = await cinema.GetSites();
                        if(sites.Where(s => s.Id == data[1]).SingleOrDefault() is Cinema.Site cinemaSite)
                        {
                            // show repertoir for the site
                            int i = 0;
                            var repertoir = await cinema.GetRepertoir(cinemaSite);
                            var buttons = repertoir
                                .Select(movie => new InlineQueryResultPhoto()
                                {
                                    Id = i++.ToString(),
                                    Title = movie.Title,
                                    Url = movie.PosterImage,
                                    ThumbUrl = movie.PosterImage,
                                    Caption = $"{msg.Query}: {movie.Title}"
                                });
                            bot.AnswerInlineQueryAsync(msg.Id, buttons.ToArray(), 0, true);
                        }
                        else if(data[1] == "upcoming")
                        {

                        }

                        // Check if has any prefered sites
                        // if not data[1] == upcoming
                        //   if yes, try to match data[1] to repertoire and/or upcoming
                        //   if not, try to match data[1] to sites
                        //     if success, show repertoire, ignore upcoming
                        // else check preferences, and show upcoming listings
                    }
                    else if (data.Length == 3) // == <end param count>
                    {
                        var infos = await cinema.FindSeance(data[1], data[2]);
                        var res = ButtonHelper.CreateInlineQuery(infos);
                        bot.AnswerInlineQueryAsync(msg.Id, res.ToArray(), 0);
                    }
                }
            }
            else
                bot.AnswerInlineQueryAsync(msg.Id, null, 0, switchPmText: "Test", switchPmParameter: "Test2");
        }

        private void Bot_OnReceiveError(object sender, ReceiveErrorEventArgs e)
        {
            bot.SendTextMessageAsync(PrivateChatID, $"API error: {e.ApiRequestException}\n");
        }
        private void Bot_OnReceiveGeneralError(object sender, ReceiveGeneralErrorEventArgs e)
        {
            bot.SendTextMessageAsync(PrivateChatID, $"General error: {e.Exception}\n");
            bShouldIgnore = true;
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

        public void Check(object sender, System.Timers.ElapsedEventArgs e)
        {
            //checker = new CinemaChecker();
            //if (RequestId == null)
            //{
            //    foreach (var settings in ChatSettings)
            //    {
            //        var poster = checker.GetPosters()
            //            .Where(post => settings.Value.Contains(post.Title)).FirstOrDefault();

            //        if (poster != null)
            //        {
            //        //    var sites = checker.GetFeatured(poster.Code)
            //        //        .Where(feat => settings.Value.Contains(feat.Id))
            //        //        .Select(feat => feat.Name);
                    
            //            bot.SendTextMessageAsync(settings.Key, $"Hey, the movie you were waiting on ('{poster.Title}') is now available!");
            //        }
            //    }
            //}
            //else
            //{
            //    var settings = GetChatSettings(RequestId);
            //    var poster = checker.GetPosters()
            //        .Where(post => settings.Contains(post.Title)).FirstOrDefault();

            //    if (poster != null)
            //    {
            //        var sites = checker.GetFeatured(poster.Code)
            //            .Where(feat => settings.Contains(feat.Id))
            //            .Select(feat => feat.Name);

            //        bot.SendTextMessageAsync(RequestId, $"Hey, the movie you were waiting on ('{poster.Title}') is now available!");
            //    }
            //}
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            SaveSettings();
            bot.StopReceiving();
            if (disposing)
            {

            }
        }

        private string BotUsername => MeResult.Result.Username;

        const long PrivateChatID = 0x28d07dc;
        const string Bot_SelectSite = "Select cinema site";
        const string Bot_SelectCinemaSystem = "Select cinema system";
        static bool bShouldIgnore = false;

        private InlineKeyboardButton[][] Bot_SettingsInlineKeyboard = new[]
        {
            new[]
            {
                new InlineKeyboardCallbackButton("Registered sites", "bot_showsites")
            },
            new[]
            {
                new InlineKeyboardCallbackButton("Seance preferences", "bot_showprefs")
            }
        };
    }
}
