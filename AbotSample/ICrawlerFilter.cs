using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CliClap.Web.Serivces.API.Models
{
    public interface ICrawlerFilter<T>
    {
        string Expression { get; }
        bool Execute(T Input);        
    }

    public interface ICrawlerFilter : ICrawlerFilter<string> { }
}