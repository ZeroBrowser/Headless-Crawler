using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Frontier;

namespace ZeroBrowser.Crawler.Api.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlerController : ControllerBase
    {
        private readonly ILogger<CrawlerController> _logger;
        private readonly IBackgroundUrlQueue _backgroundUrlQueue;
        private readonly FrontierState _frontierState;

        public CrawlerController(ILogger<CrawlerController> logger, IBackgroundUrlQueue backgroundUrlQueue, FrontierState frontierState)
        {
            _logger = logger;
            _backgroundUrlQueue = backgroundUrlQueue;
            _frontierState = frontierState;
        }

        [HttpGet()]
        public IActionResult Get()
        {
            var urls = _frontierState.ProcessedUrls.Select(a => a.Key);

            return Ok(urls);
        }

        [HttpGet("/api/Crawler/getstructureddata")]
        public IActionResult GetStructuredData()
        {
            var response = _frontierState.CrawledTree;

            return Ok(new { response?.Root?.Parent, response?.Root?.Value, response?.Root?.Children });
        }


        /// <summary>
        /// Start crawler
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///         "seedUrls" : ["https://www.0browser.com"],
        ///         "headlessBrowserUrl" : "wss://proxy.0browser.com?token="
        ///     }
        ///
        /// </remarks>
        /// <param name="item"></param>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Recieved the seed url without any issues</response>
        /// <response code="500">Something wrong!</response>            
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Post([FromBody] Parameters parameter)
        {
            _logger.LogInformation($"* url recieved {parameter.SeedUrls.First()}{Environment.NewLine}");

            _backgroundUrlQueue.EnqueteUrlItem(parameter.SeedUrls.First(), isSeed: true);

            return Ok();
        }
    }
}
