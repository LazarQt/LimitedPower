using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using LimitedPower.DraftHelperSync.Extensions;

namespace LimitedPower.DraftHelperSync
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting sync...");

            Console.WriteLine("cookie?");
            var cookie = Console.ReadLine();

            // get 17lands stuff
            var today = DateTime.Now;
            var url =
                $"https://www.17lands.com/card_ratings/data?expansion=MID&format=PremierDraft&start_date=2021-09-15&end_date={today.Year}-{today.Month:00}-{today.Day:00}";
            var doc = new System.Net.WebClient().DownloadString(Uri.EscapeUriString(url));
            var cardRatings = JsonSerializer.Deserialize<List<SeventeenLandsEvaluation>>(doc);
            if (cardRatings == null)
            {
                Console.WriteLine("could not load 17lands data");
                return;
            };

            // load locally downloaded mtgahelper stuff 
            var mtgaHelperCards = JsonSerializer.Deserialize<List<MtgaHelperEvaluation>>(System.IO.File.ReadAllText($"customDraftRatingsForDisplay.json"));
            if (mtgaHelperCards == null)
            {
                Console.WriteLine("could not read local mtgahelper data");
                return;
            };

            // setup client
            var client = new RestClient("https://mtgahelper.com/api/User/CustomDraftRating")
            {
                Timeout = -1,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:92.0) Gecko/20100101 Firefox/92.0"
            };

            // math mumbo jumbo
            var lowestPick = cardRatings.Min(c => c.AvgSeen);
            var highestPick = cardRatings.Max(c => c.AvgSeen);

            // execute requests
            foreach (var cardRating in cardRatings)
            {
                var mtgaHelperCard = mtgaHelperCards.FirstOrDefault(m => m.Card.Name == cardRating.Name);
                if (mtgaHelperCard == null)
                {
                    Console.WriteLine($"can not find {cardRating.Name}");
                    continue;
                }

                if (cardRating.Name != "Plummet") continue;

                var request = new RestRequest(Method.PUT);
                var note = $"WR: {cardRating.EverDrawnWinRate.ToStringValue(0)}% | IWD: {cardRating.DrawnImprovementWinRate.ToStringValue(2)}pp (v2)";
                request.Generate(@"{idArena:" + mtgaHelperCard.Card.IdArena + ",note:" + "\"" + note + "\"" + ",rating:" + cardRating.AvgSeen.TransformRating(lowestPick, highestPick) + "}", cookie);
                //client.ExecuteAsync(request);
                //Console.WriteLine("executed request for " + cardRating.Name);
                var response = client.Execute(request);
                //Console.WriteLine(response.Content);
                Console.WriteLine("executed request for " + cardRating.Name);
            }


        }

    }
}
