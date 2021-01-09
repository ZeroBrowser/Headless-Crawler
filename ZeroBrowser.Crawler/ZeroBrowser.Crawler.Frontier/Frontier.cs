using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Frontier
{
    public class Frontier : IFrontier
    {
        //key is url and value is number of times this url is pass in to Frontier
        private readonly ConcurrentDictionary<string, int> _crawledUrls;
        private readonly ConcurrentQueue<string> _queue;
        private readonly IUrlChannel _urlProducer;

        public Frontier(IUrlChannel urlProducer)
        {
            _crawledUrls = new ConcurrentDictionary<string, int>();
            _queue = new ConcurrentQueue<string>();
            _urlProducer = urlProducer;
        }

        public async Task Process(string[] urls)
        {
            foreach (var url in urls)
            {
                if (_crawledUrls.ContainsKey(url))
                {
                    //stop

                    _crawledUrls[url]++;
                    continue;
                }
                else
                {
                    //add to queue

                    _crawledUrls.TryAdd(url, 1);
                    //_queue.Enqueue(url);
                    await _urlProducer.Insert(url);
                }
            }

        }
    }
}
