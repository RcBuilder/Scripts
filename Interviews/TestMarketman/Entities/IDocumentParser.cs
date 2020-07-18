using HtmlAgilityPack;

namespace Entities
{
    public interface IDocumentParser<T> : IParser<HtmlDocument, T> { }
}
