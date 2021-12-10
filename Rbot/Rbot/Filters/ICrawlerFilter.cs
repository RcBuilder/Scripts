
namespace Crawler.Models
{
    public interface ICrawlerFilter<T>
    {
        string Expression { get; }
        bool Execute(T Input);        
    }

    public interface ICrawlerFilter : ICrawlerFilter<string> { }
}