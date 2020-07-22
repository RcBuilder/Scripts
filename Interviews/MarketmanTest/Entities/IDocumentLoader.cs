using HtmlAgilityPack;
using System.Threading.Tasks;

namespace Entities
{
    public interface IDocumentLoader
    {
        HtmlDocument Load(string URL);        
        HtmlDocument LoadHtml(string Input);
    }

    public interface IAsyncDocumentLoader : IDocumentLoader
    {
        Task<HtmlDocument> LoadAsync(string URL);        
    }
}
