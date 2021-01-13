﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;

namespace ZeroBrowser.Crawler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> _logger;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;

        public CrawlController(ILogger<CrawlController> logger, IBackgroundUrlQueue backgroundUrlQueue)
        {
            _logger = logger;
            _backgroundUrlQueue = backgroundUrlQueue;
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
