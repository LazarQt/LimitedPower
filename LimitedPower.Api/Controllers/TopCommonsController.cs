using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using LimitedPower.Model;
using Microsoft.AspNetCore.Cors;
using Newtonsoft.Json;

namespace LimitedPower.Api.Controllers
{
    [ApiController]
    [EnableCors("LocalCrossPolicy")]
    [Route("[controller]")]
    public class TopCommonsController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<TierListController> _logger;

        public TopCommonsController(ILogger<TierListController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{setCode}")]
        public List<ViewCard> Get(string setCode, bool live, string callParams)
        {
            var cards = JsonConvert.DeserializeObject<List<ViewCard>>(System.IO.File.ReadAllText($"Set/{setCode}.json"));
            if (cards == null) return null;
            cards = live ? cards.OrderByDescending(c => c.LiveRating).ToList() : cards.OrderByDescending(c => c.InitialRating).ToList();

            var commons = cards.Where(x => x.Rarity == "common").ToList();
            var bestCommons = new List<ViewCard>();
            foreach (var p in callParams.Split(','))
            {
                var colors = p;
                var take = 5;
                if (p.Contains("-"))
                {
                    var splitP = p.Split('-');
                    colors = splitP[0];
                    take = Convert.ToInt32(splitP[1]);
                }
                var x = commons.Where(i => i.GetSortedString().ToLower() == string.Concat(colors.OrderBy(c => c)).ToLower());
                bestCommons.AddRange(x.Take(take));
            }
            return bestCommons;
        }
    }
}
