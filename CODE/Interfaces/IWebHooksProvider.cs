using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entities
{
    public interface IWebHooksProvider<T>
    {        
        IEnumerable<T> GetDataUpdates(DataUpdatesConfig config);
    }

    public interface IWebHooksProviderAsync<T> : IWebHooksProvider<T>
    {
        Task<IEnumerable<T>> GetDataUpdatesAsync(DataUpdatesConfig config);
    }    
}
