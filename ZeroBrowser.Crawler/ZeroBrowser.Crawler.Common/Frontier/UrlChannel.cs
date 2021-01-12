using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using ZeroBrowser.Crawler.Common.Interfaces;

namespace ZeroBrowser.Crawler.Common.Frontier
{
    public class UrlChannel : IUrlChannel
    {
        private readonly Channel<string> _channel;
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public UrlChannel()
        {
            _channel = Channel.CreateUnbounded<string>();
        }

        public async Task Insert(string url)
        {
            await _channel.Writer.WriteAsync(url);

            _signal.Release();
        }

        public async Task<IAsyncEnumerable<string>> Read()
        {
            await _signal.WaitAsync();
            return _channel.Reader.ReadAllAsync();
        }
    }
}
