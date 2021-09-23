
using System.Threading.Tasks;

namespace Entities
{
    public interface IFactory<TIn, TOut>
    {
        TOut Produce(TIn metadata);
    }

    public interface IFactoryAsync<TIn, TOut> : IFactory<TIn, TOut>
    {
        Task<TOut> ProduceAsync(TIn metadata);
    }
}
