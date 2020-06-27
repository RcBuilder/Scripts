using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ServiceStack.Redis;

/*
    NOTE! free quota is limited to '6000 Redis requests per hour 
*/

namespace AWSCommon.Redis
{
    internal class ServiceStackRedisClient : IRedisClient
    {
        private RedisClient client = null;

        public ServiceStackRedisClient(string EndPoint, int Port)
        {
            this.client = new RedisClient(EndPoint, Port);
        }

        public bool Remove(string Key)
        {
            if (string.IsNullOrEmpty(Key))
                return false;
            return this.client.Remove(Key);
        }

        public bool Set<T>(string Key, T Value, DateTime? ExpiryDate = null)
        {
            if (string.IsNullOrEmpty(Key))
                return false;

            if (!ExpiryDate.HasValue)
                return this.client.Set<T>(Key, Value);
            return this.client.Set<T>(Key, Value, ExpiryDate.Value);
        }

        public T Get<T>(string Key)
        {
            if (string.IsNullOrEmpty(Key))
                return default(T);
            return this.client.Get<T>(Key);
        }

        public string GetAsJson(string Key) { throw new NotImplementedException(); }

        public IEnumerable<T> GetAll<T>()
        {
            var keys = this.client.GetAllKeys();
            return GetMultiple<T>(keys);
        }

        public IEnumerable<T> GetMultiple<T>(IEnumerable<string> Keys)
        {
            var entries = this.client.GetAll<T>(Keys);

            if (entries == null)
                return null;
            return entries.Values;
        }

        public IEnumerable<string> GetMultipleAsJson(IEnumerable<string> Keys) { throw new NotImplementedException(); }

        public IEnumerable<string> GetAllKeys()
        {
            return this.client.GetAllKeys();
        }

        public IEnumerable<string> SearchKeys(string Pattern)
        {
            if (string.IsNullOrEmpty(Pattern))
                return null;
            return this.client.SearchKeys(string.Format("*{0}*", Pattern));
        }

        long IRedisClient.AddToList<T>(string Key, T Value) { throw new NotImplementedException(); }
        long IRedisClient.AddToList<T>(string Key, IEnumerable<T> Values) { throw new NotImplementedException(); }
        public T GetFromList<T>(string Key, long ItemIndex) { throw new NotImplementedException(); }
        public IEnumerable<T> GetTopXFromList<T>(string Key, int Quantity) { throw new NotImplementedException(); }
        public bool RemoveFromList<T>(string Key, T ItemValue) { throw new NotImplementedException(); }
        public bool AddToUniqueList<T>(string Key, T Value) { throw new NotImplementedException(); }
        public bool AddToUniqueList<T>(string Key, IEnumerable<T> Values) { throw new NotImplementedException(); }
        public IEnumerable<T> GetTopXFromUniqueList<T>(string Key, int Quantity) { throw new NotImplementedException(); }
        public bool RemoveFromUniqueList<T>(string Key, T ItemValue) { throw new NotImplementedException(); }
        public bool RemoveFromUniqueList<T>(string Key, IEnumerable<T> ItemsValue) { throw new NotImplementedException(); }
        public bool MakeTransaction(params RedisTransactionAction[] actions) { throw new NotImplementedException(); }
        public bool MakeTransactionWithResults(out List<RedisTransactionAction> results, params RedisTransactionAction[] actions) { throw new NotImplementedException(); }
        public void FlushALL() { throw new NotImplementedException(); }

        public void Dispose()
        {
            this.client.Dispose();
        }
    }
}
