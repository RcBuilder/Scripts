using Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Scrapper
{
    public abstract class Scrapper<T> : IAsyncScrapper<T>
    {
        protected IDocumentLoader DocumentLoader { get; set; }

        public T Value { get; set; }        
        
        public Scrapper(IDocumentLoader DocumentLoader) {
            this.DocumentLoader = DocumentLoader;
        }

        public void Run() {
            var documet = this.Load();
            this.Parse(documet);
        }

        public async Task RunAsync()
        {
            var documet = await this.LoadAsync();
            this.Parse(documet);
        }

        public abstract HtmlDocument Load();
        public abstract Task<HtmlDocument> LoadAsync();
        public abstract void Parse(HtmlDocument Document);
    }
}
