using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;
using ZeroBrowser.Crawler.Frontier;

namespace ZeroBrowser.Crawler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> _logger;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private readonly FrontierState _frontierState;

        public CrawlController(ILogger<CrawlController> logger, IBackgroundUrlQueue backgroundUrlQueue, FrontierState frontierState)
        {
            _logger = logger;
            _backgroundUrlQueue = backgroundUrlQueue;
            _frontierState = frontierState;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var urls = _frontierState.CrawledUrls.Select(a => a.Key);

            return Ok(urls);
        }


        [HttpPost]
        public IActionResult Post([FromBody] Parameters parameter)
        {
            _logger.LogInformation($"* url recieved {parameter.SeedUrls.First()}{Environment.NewLine}");

            _backgroundUrlQueue.QueueUrlItem(parameter.SeedUrls.First(), true);

            return Ok();
        }
    }
}
