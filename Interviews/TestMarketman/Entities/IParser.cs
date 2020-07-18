
namespace Entities
{
    public interface IParser<TIn, TOut>
    {
        TOut Parse(TIn Input);
    }
}
