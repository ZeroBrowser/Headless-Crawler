using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Common.Frontier
{
    public class UrlChannel : IUrlChannel
    {
        private readonly Channel<string> _channel;

        private UrlChannel()
        {
            _channel = Channel.CreateUnbounded<string>();
        }

        public async Task Insert(string url)
        {
            await _channel.Writer.WriteAsync(url);
        }

        public IAsyncEnumerable<string> Read()
        {
            return _channel.Reader.ReadAllAsync();
        }
    }
}
