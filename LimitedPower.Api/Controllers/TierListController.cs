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
    public class TierListController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<TierListController> _logger;

        public TierListController(ILogger<TierListController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{setCode}")]
        public List<ViewCard> Get(string setCode, bool live)
        {
            var cards = JsonConvert.DeserializeObject<List<ViewCard>>(System.IO.File.ReadAllText($"Set/{setCode}.json"));
            if (cards == null) return null;
            cards = live ? cards.OrderByDescending(c => c.LiveRating).ToList() : cards.OrderByDescending(c => c.InitialRating).ToList();
            return cards;
        }
    }
}
