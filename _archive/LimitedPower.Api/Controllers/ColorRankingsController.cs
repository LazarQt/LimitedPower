using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Cors;

namespace LimitedPower.Api.Controllers
{
    [ApiController]
    [EnableCors("LocalCrossPolicy")]
    [Route("[controller]")]
    public class ColorRankingsController : ControllerBase
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger<TierListController> _logger;

        public ColorRankingsController(ILogger<TierListController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{setCode}")]
        public Dictionary<string, double> Get(string setCode, bool live, string callParams)
        {
            return new ColorRankingsCalculator().GetRankings(setCode, live, callParams);
        }
    }
}
