using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace CinemaChecker
{
    class ButtonHelper
    {
        public static IEnumerable<InlineQueryResultBase> CreateInlineQuery(List<Cinema.SeanceInfo> seanceinfo)
        {
            int i = 0;
            foreach (var info in seanceinfo)
            {
                var movie = info.Movie;
                var reply = CreateSeanceReply(info.Seances);
                yield return new InlineQueryResultArticle(i++.ToString(), movie.Title, new InputTextMessageContent(movie.Title))
                {
                    Description = info.Type,
                    ThumbUrl = movie.PosterImage,
                    ReplyMarkup = new InlineKeyboardMarkup(reply.Partition(2).ToArray()),
                };
            }
        }
        public static IEnumerable<InlineKeyboardButton> CreateSeanceReply(List<Cinema.Seance> seances)
        {
            foreach (var seance in seances)
            {
                yield return InlineKeyboardButton.WithCallbackData(seance.Date.ToString(), "TestCallback");
            }
        }
    }
}
