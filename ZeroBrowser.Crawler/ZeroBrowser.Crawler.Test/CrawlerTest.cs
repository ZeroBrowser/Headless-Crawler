using System;
using Xunit;
using ZeroBrowser.Crawler.Core;

namespace ZeroBrowser.Crawler.Test
{
    public class CrawlerTest
    {
        [Fact]
        public void Test1()
        {
            //Given
            var seedUrl = "https://www.0browser.com";


            //When
            var crawler = new Core.Crawler(new[] { seedUrl });

            //Then
            
        }
    }
}
