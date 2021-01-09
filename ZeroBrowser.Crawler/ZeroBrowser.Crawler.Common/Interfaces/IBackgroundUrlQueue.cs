using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IBackgroundUrlQueue
    {
        void QueueUrlItem(string url);

        Task<string> DequeueAsync(CancellationToken cancellationToken);
    }
}
