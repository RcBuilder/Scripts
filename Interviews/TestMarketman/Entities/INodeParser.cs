using HtmlAgilityPack;

namespace Entities
{
    public interface INodeParser<T> : IParser<HtmlNode, T> { }
}
