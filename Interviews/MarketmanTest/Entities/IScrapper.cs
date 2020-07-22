using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public interface IScrapper<T>
    {
        T Value { get; set; }
        void Run();
    }

    public interface IAsyncScrapper<T> : IScrapper<T> {
        Task RunAsync();
    }
}
