using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineKeyboardButtons;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputMessageContents;
using Telegram.Bot.Types.ReplyMarkups;

namespace CinemaChecker
{
    class ButtonHelper
    {
        public static IEnumerable<InlineQueryResult> CreateInlineQuery(List<Cinema.SeanceInfo> seanceinfo)
        {
            int i = 0;
            foreach (var info in seanceinfo)
            {
                var movie = info.Movie;
                var reply = CreateSeanceReply(info.Seances);
                yield return new InlineQueryResultArticle()
                {
                    Id = i++.ToString(),
                    Title = movie.Title,
                    Description = info.Type,
                    ThumbUrl = movie.PosterImage,
                    ReplyMarkup = new InlineKeyboardMarkup(reply.Partition(2).ToArray()),
                    InputMessageContent = new InputTextMessageContent()
                    {
                        MessageText = movie.Title
                    }
                };
            }
        }
        public static IEnumerable<InlineKeyboardButton> CreateSeanceReply(List<Cinema.Seance> seances)
        {
            foreach (var seance in seances)
            {
                yield return new InlineKeyboardCallbackButton(seance.Date.ToString(), "TestCallback");
            }
        }
    }
}
