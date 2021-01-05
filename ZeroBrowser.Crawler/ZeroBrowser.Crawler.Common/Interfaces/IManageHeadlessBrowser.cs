using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ZeroBrowser.Crawler.Common.Interfaces
{
    public interface IManageHeadlessBrowser
    {
        Task Init();
        Task<T> GetPage<T>(int index) where T : class;

        Task ClosePage(int index);
    }
}
