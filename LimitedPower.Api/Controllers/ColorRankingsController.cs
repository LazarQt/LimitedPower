using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LimitedPower.Model;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace LimitedPower.Api.Controllers
{
    [ApiController]
    [EnableCors("AnotherPolicy")]
    [Route("[controller]")]
    public class ColorRankingsController : ControllerBase
    {
        private readonly ILogger<TierListController> _logger;

        public ColorRankingsController(ILogger<TierListController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{setCode}")]
        public Dictionary<string, double> Get(string setCode, bool live, string callParams)
        {
            var cards = JsonConvert.DeserializeObject<List<ViewCard>>(System.IO.File.ReadAllText($"Set/{setCode}.json"));
            if (cards == null) return null;
            cards = live ? cards.OrderByDescending(c => c.LiveRating).ToList() : cards.OrderByDescending(c => c.InitialRating).ToList();

            var colors = new Dictionary<string, List<ViewCard>>()
            {
                {"w", new List<ViewCard>()},
                {"u", new List<ViewCard>()},
                {"b", new List<ViewCard>()},
                {"r", new List<ViewCard>()},
                {"g", new List<ViewCard>()}
            };

            foreach (var color in colors)
            {
                foreach (var card in cards)
                {
                    if (card.CanBeCastWithOnly(color.Key))
                    {
                        color.Value.Add(card);
                    }
                }
            }

            var rankings = new Dictionary<string, double>() { };
            foreach (var x in colors)
            {
                var r = live ? x.Value.Average(u => u.LiveRating) : x.Value.Average(u => u.InitialRating);
                r = Math.Round(r, 2);
                rankings.Add(x.Key, r);
            }

            return rankings;


            //var commons = cards.Where(x => x.Rarity == "common").ToList();
            //var bestCommons = new List<ViewCard>();
            //foreach (var p in callParams.Split(','))
            //{
            //    var x = commons.Where(i => i.Colors.GetString().ToLower() == String.Concat(p.OrderBy(c => c)).ToLower());
            //    bestCommons.AddRange(x.Take(5));
            //}
            //return bestCommons;
        }
    }
}
