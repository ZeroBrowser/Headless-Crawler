using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core.Interfaces;

namespace ZeroBrowser.Crawler.Core
{
    public class Frontier : IFrontier
    {
        private readonly IRepository _repository;

        public Frontier(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<string>> Process(IEnumerable<WebPage> webPages)
        {
            var pagesToCrawl = new List<string>();            

            foreach (var webPage in webPages)
            {
                //check if we already crawled this page
                if (!await _repository.Exist(webPage.Url))
                {
                    pagesToCrawl.Add(webPage.Url);
                }
            }
            
            return pagesToCrawl;
        }
    }
}
