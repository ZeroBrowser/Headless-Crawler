using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;
using ZeroBrowser.Crawler.Common.Models;

namespace ZeroBrowser.Crawler.Common.Channels
{
    public class UrlChannel : IUrlChannel
    {
        private readonly Channel<CrawlerContext> _channel;
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public UrlChannel()
        {
            _channel = Channel.CreateUnbounded<CrawlerContext>();
        }

        public async Task Insert(CrawlerContext crawlerContext)
        {
            await _channel.Writer.WriteAsync(crawlerContext);

            _signal.Release();
        }

        public async Task<IAsyncEnumerable<CrawlerContext>> Read()
        {
            await _signal.WaitAsync();
            return _channel.Reader.ReadAllAsync();
        }
    }
}
