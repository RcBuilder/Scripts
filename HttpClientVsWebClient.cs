﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    /*
        var comparer = new HttpClientVsWebClient();

        Console.WriteLine("UseHttpClient");
        var resultUseHttpClient = comparer.UseHttpClient();
        Console.WriteLine("Time: {0}", resultUseHttpClient);
     
        Console.WriteLine("UseWebClient");
        var resultUseWebClient = comparer.UseWebClient();
        Console.WriteLine("Time: {0}", resultUseWebClient);

        Console.WriteLine("UseHttpClient2");
        var resultUseHttpClient2 = comparer.UseHttpClient2();
        Console.WriteLine("Time: {0}", resultUseHttpClient2);

        Console.WriteLine("[Results] UseWebClient {0:F} sec | UseHttpClient {1:F} sec | UseHttpClient2 {1:F} sec", resultUseWebClient, resultUseHttpClient, resultUseHttpClient2); 
    */
    public class HttpClientVsWebClient : IDisposable
    {
        private readonly string domain = "http://example.com/";
        private readonly int counter = 200;
        private List<string> sites { set; get; }

        /*
        private HttpClient httpClient { set; get; }
        private WebClient webClient { set; get; }
        */



        public HttpClientVsWebClient() {
            // load sites
            /// var sites = Enumerable.Repeat<string>(this.domain, this.counter).ToList();
            this.sites = Enumerable.Range(1, this.counter).Select(x => this.domain + "?p=" + x).ToList();

            /*
            this.httpClient = new HttpClient();
            this.webClient = new WebClient();
            */
        }

        public double UseHttpClient() {
            var tasks = new List<Task<string>>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // also the sites loop is parallel
            this.sites.AsParallel().ForAll(site => {
                Console.WriteLine(site);
                tasks.Add(ReadSiteAsync(site));
            });

            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();            
            return stopwatch.Elapsed.TotalSeconds;
        }

        public double UseHttpClient2()
        {
            var tasks = new List<Task<string>>();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // sites loop is NOT parallel
            this.sites.ForEach(site => {
                Console.WriteLine(site);
                tasks.Add(ReadSiteAsync(site));
            });

            Task.WaitAll(tasks.ToArray());

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalSeconds;
        }

        public double UseWebClient()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            this.sites.ForEach(site => {
                Console.WriteLine(site);
                ReadSiteSync(site);
            });

            stopwatch.Stop();
            return stopwatch.Elapsed.TotalSeconds;
        }

        public async Task<string> ReadSiteAsync(string site)
        {
            var result = string.Empty;

            var clientHandler = new HttpClientHandler {
                UseProxy = false
            };

            using (var client = new HttpClient(clientHandler, false))
            using (var resonse = await client.GetAsync(site).ConfigureAwait(false))
                result =  await resonse.Content.ReadAsStringAsync();

            var unique = Guid.NewGuid().ToString();
            Console.WriteLine(unique);

            return unique;
        }

        public string ReadSiteSync(string site)
        {
            var result = string.Empty;
            using (var client = new WebClient())
                result = client.DownloadString(site);

            var unique = Guid.NewGuid().ToString();
            Console.WriteLine(unique);

            return unique;
        }

        public void Dispose()
        {
            /*
            this.httpClient.Dispose();
            this.webClient.Dispose();
            */
        }
    }
}
