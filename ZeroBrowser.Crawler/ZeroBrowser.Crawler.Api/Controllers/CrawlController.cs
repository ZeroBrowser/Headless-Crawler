using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;

namespace ZeroBrowser.Crawler.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlController : ControllerBase
    {        
        private readonly ILogger<CrawlController> _logger;
        private readonly ICrawler _crawler;

        public CrawlController(ILogger<CrawlController> logger, ICrawler crawler)
        {
            _logger = logger;
            _crawler = crawler;
        }

        [HttpPost]
        public async Task Post([FromBody]Parameters parameter)
        {
            await _crawler.Crawl(parameter.SeedUrls.First());
        }
    }
}
