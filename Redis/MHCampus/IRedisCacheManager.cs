using CommonEntities.Redis;
using System;
using System.Collections.Generic;
namespace AWSCommon.Redis
{
    //Design Pattern: Facade.
    //Facade is to a layer that 
    //(a) accesses a redis instance's data structures
    //(b) applies typing and serialization to the values entered and obtained from redis
    //(c) encapsulates the underlying .net library that is used to access redis (so that in can be changed in future if need be)
    //(d) enables redis transactions to be abstractly defined no matter which underlying .net library, .net serializer is used.
    public interface IRedisCacheManager: IDisposable
    {        
        bool Set<T>(ICacheKey key, T value, DateTime? ExpiryDate = null);
        T Get<T>(ICacheKey key);
        string GetAsJson(ICacheKey key);
        IEnumerable<T> GetMultiple<T>(IEnumerable<ICacheKey> keys);
        IEnumerable<string> GetMultipleAsJson(IEnumerable<ICacheKey> keys);   
        IEnumerable<T> GetALL<T>();
        bool AddToUniqueList<T>(ICacheKey Key, T Value);
        bool AddToUniqueList<T>(ICacheKey key, T[] Values);        
        IEnumerable<T> GetTopXFromUniqueList<T>(ICacheKey key, int numToTake);
        IEnumerable<T> GetAllFromUniqueList<T>(ICacheKey key);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> SearchKeys(string pattern);
        bool Remove(ICacheKey key);
        bool RemoveFromUniqueList<T>(ICacheKey key, T ItemValue);
        bool MakeTransaction(params RedisTransactionAction[] actions);
        RedisTransactionActionWithResult<T> CreateTxnGetAction<T>(eRedisTransactionActionType actionType, ICacheKey key);
        bool GetAndRemove<T>(ICacheKey key, out T responseObj);
    }
}
