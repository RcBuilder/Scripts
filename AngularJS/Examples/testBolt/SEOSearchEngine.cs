using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public abstract class SEOSearchEngine : Entities.ISEOSearchEngine
    {
        public abstract IEnumerable<Entities.SearchResult> SearchTopX(string Phrase, int rowcount);

        protected string GetPageSource(string baseURL, string Phrase) {
            var pageSource = string.Empty;

            using (var client = new WebClient())
            {
                client.Proxy = null;
                client.Encoding = Encoding.UTF8;
                return client.DownloadString(string.Format(baseURL, HttpUtility.UrlEncode(Phrase)));
            }
        }

        protected string GetPageSourceUsingDriver(string baseURL, string Phrase) {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless"); // no UI
            chromeOptions.AddArguments("blink-settings=imagesEnabled=false"); // disable images download
            
            var browser = new ChromeDriver(chromeOptions);
            browser.Navigate().GoToUrl(string.Format(baseURL, HttpUtility.UrlEncode(Phrase)));
            var content = browser.PageSource;
            Thread.Sleep(1000); // TODO ->> replace with waitHandler

            browser.Close();
            browser.Dispose();

            return content;
        }
    }
}
