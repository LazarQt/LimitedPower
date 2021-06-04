﻿using Microsoft.AspNetCore.Mvc;
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
    public class TierListController : ControllerBase
    {
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