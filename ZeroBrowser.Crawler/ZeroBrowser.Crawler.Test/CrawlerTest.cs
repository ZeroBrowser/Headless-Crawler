using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using ZeroBrowser.Crawler.Common.Models;
using ZeroBrowser.Crawler.Core;

namespace ZeroBrowser.Crawler.Test
{
    public class CrawlerTest
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task CheckSeedUrl_Null_ThowException_Test()
        {
            //Given            
            var moq = new Mock<IOptions<CrawlerOptions>>();
            string headlessBrowserUrl = null;
            var crawlerContext = new CrawlerContext() { CurrentUrl = headlessBrowserUrl, IsSeed = true };

            //When
            var crawler = new Core.Crawler(null, null, moq.Object, null);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => crawler.Crawl(crawlerContext));

            Assert.Equal("Null/Empty", ex.Message);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(" ")]
        [InlineData("")]
        public async Task CheckSeedUrl_Empty_ThowException_Test(string seedUrl)
        {
            //Given            
            var moq = new Mock<IOptions<CrawlerOptions>>();
            var crawlerContext = new CrawlerContext() { CurrentUrl = seedUrl, IsSeed = true };

            //When
            var crawler = new Core.Crawler(null, null, moq.Object, null);
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => crawler.Crawl(crawlerContext));

            Assert.Equal("Null/Empty", ex.Message);
        }


        //[Theory]
        //[Trait("Category", "Unit")]
        //[InlineData("http://www..com", "Invalid uri")]
        //[InlineData("httpp://www.", "Invalid scheme")]
        //[InlineData("httpp://", "Invalid scheme")]
        //[InlineData(".com", "Invalid uri")]
        //[InlineData("abc", "Invalid uri")]
        //[InlineData("ftp://url", "Invalid scheme")]
        //[InlineData("wss://url", "Invalid scheme")]
        //public async Task CheckSeedUrl_ValidURL_ThowException_Test(string seedUrl, string expectedError)
        //{
        //    //Given            
        //    var headlessBrowserUrl = "http://test";

        //    //When
        //    var crawler = new Core.Crawler(new[] { seedUrl }, headlessBrowserUrl, null);
        //    var ex = await Assert.ThrowsAsync<ArgumentException>(() => crawler.Crawl());

        //    Assert.Equal($"{expectedError} (Parameter 'SeedUrls')", ex.Message);
        //}


        //[Fact]
        //[Trait("Category", "Unit")]
        //public async Task CheckHeadlessBrowserUrl_Null_ThowException_Test()
        //{
        //    //Given            
        //    var seedUrls = new[] { "http://test" };

        //    //When
        //    var crawler = new Core.Crawler(seedUrls, null, null);
        //    var ex = await Assert.ThrowsAsync<ArgumentException>(() => crawler.Crawl());

        //    //Then
        //    Assert.Equal("The HeadlessBrowserUrl field is required. (Parameter 'HeadlessBrowserUrl')", ex.Message);
        //}

        //[Theory]
        //[Trait("Category", "Unit")]
        //[InlineData("http://www..com", "Invalid uri")]
        //[InlineData("httpp://www.", "Invalid scheme")]
        //[InlineData("httpp://", "Invalid scheme")]
        //[InlineData(".com", "Invalid uri")]
        //[InlineData("abc", "Invalid uri")]
        //[InlineData("ftp://url", "Invalid scheme")]
        //[InlineData("http://www.google.com", "Invalid scheme")]
        //public async Task CheckHeadlessBrowserUrl_BadUrl_ThowException_Test(string headlessBrowserUrl, string expectedError)
        //{
        //    //Given            
        //    var seedUrls = new[] { "http://test" };

        //    //When
        //    var crawler = new Core.Crawler(seedUrls, headlessBrowserUrl, null);
        //    var ex = await Assert.ThrowsAsync<ArgumentException>(() => crawler.Crawl());

        //    Assert.Equal($"{expectedError} (Parameter 'HeadlessBrowserUrl')", ex.Message);
        //}

        //[Fact]
        //[Trait("Category", "Unit")]
        //public async Task CheckHeadlessBrowserUrl_Valid_Test()
        //{
        //    //Given            
        //    var seedUrls = new[] { "http://www.0browser.com" };
        //    var headlessBrowserUrl = "wss://proxy.0browser.com";

        //    //When
        //    var crawler = new Core.Crawler(seedUrls, headlessBrowserUrl, null);
        //    await crawler.Crawl();

        //    //Then
        //    //Should get this far
        //}
    }
}
