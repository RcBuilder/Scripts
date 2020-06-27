using Amazon;
using Amazon.ElastiCache;
using CommonEntities.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Tegrity.Utils;

namespace AWSCommon.Redis
{
    public class RedisCacheManager: IDisposable, IRedisCacheManager
    {
        private IRedisClient redisClient;
        private static Logger logger = Logger.GetLogger("AWSCommon.Redis.RedisCacheManager");

        public RedisCacheManager()
        {
            try
            {
                //this.redisClient = new ServiceStackRedisClient(ConfigurationManager.AppSettings["RedisEndpoint"], Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]));
                this.redisClient = new StackExchangeRedisClient(ConfigurationManager.AppSettings["RedisEndpoint"], Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]));
            }
            catch
            {
                logger.Error(string.Format("Can't load redis Client: server {0}:{1}", ConfigurationManager.AppSettings["RedisEndpoint"], ConfigurationManager.AppSettings["RedisPort"]));
            }
        }

        public bool Set<T>(ICacheKey key, T value, DateTime? ExpiryDate = null)
        {
            if (key == null || value == null)
                return false;

            // Unspecified kind is causing redis to crash!!!
            if (ExpiryDate.HasValue && ExpiryDate.Value.Kind == DateTimeKind.Unspecified)
                ExpiryDate = new DateTime(ExpiryDate.Value.Ticks, DateTimeKind.Local);

            var result = this.redisClient.Set<T>(key.KeyString.ToUpper(), value, ExpiryDate);
            return result;
        }

        public T Get<T>(ICacheKey key)
        {
            if (key == null)
                throw new ArgumentNullException("parameter 'cacheKey' not supplied");// return default(T);

            var value = this.redisClient.Get<T>(key.KeyString.ToUpper());
            if (value == null)
                return default(T);

            return value;
        }

        public string GetAsJson(ICacheKey key)
        {
            if (key == null)
                throw new ArgumentNullException("parameter 'cacheKey' not supplied");// return default(T);

            var value = this.redisClient.GetAsJson(key.KeyString.ToUpper());
            if (value == null)
                return null;

            return value;
        }

        public IEnumerable<T> GetMultiple<T>(IEnumerable<ICacheKey> keys)
        {
            return this.redisClient.GetMultiple<T>(keys.Select(k => k.KeyString.ToUpper()));
        }

        public IEnumerable<string> GetMultipleAsJson(IEnumerable<ICacheKey> keys)
        {
            return this.redisClient.GetMultipleAsJson(keys.Select(k => k.KeyString.ToUpper()));
        }

        public IEnumerable<T> GetALL<T>() {
            return this.redisClient.GetAll<T>();
        }

        public bool AddToUniqueList<T>(ICacheKey Key, T Value)
        {
            if (Key == null)
                throw new ArgumentNullException("parameter 'Key' not supplied");// return default(T);
            if (Value == null)
                throw new ArgumentNullException("parameter 'Value' not supplied");// return default(T);

            return this.redisClient.AddToUniqueList<T>(Key.KeyString.ToUpper(), Value);            
        }
        public bool AddToUniqueList<T>(ICacheKey key, T[] Values)
        {
            if (key == null)
                throw new ArgumentNullException("parameter 'key' not supplied");// return default(T);
            if (Values == null)
                throw new ArgumentNullException("parameter 'Values' not supplied");// return default(T);

            return this.redisClient.AddToUniqueList<T>(key.KeyString.ToUpper(), Values);            
        }

        public IEnumerable<T> GetTopXFromUniqueList<T>(ICacheKey key, int numToTake)
        {
            if (key == null)
                throw new ArgumentNullException("parameter 'key' not supplied");// return default(T);

            return this.redisClient.GetTopXFromUniqueList<T>(key.KeyString.ToUpper(), numToTake);            
        }
        public IEnumerable<T> GetAllFromUniqueList<T>(ICacheKey key) {
            if (key == null)
                throw new ArgumentNullException("parameter 'key' not supplied");// return default(T);

            return this.redisClient.GetTopXFromUniqueList<T>(key.KeyString.ToUpper(), -1);     
        }

        public IEnumerable<string> GetAllKeys() {
            return this.redisClient.GetAllKeys();
        }

        public IEnumerable<string> SearchKeys(string pattern) {
            if (pattern == null || pattern.Trim() == string.Empty)
                return new List<string>();

            return this.redisClient.SearchKeys(string.Format("*{0}*", pattern.ToUpper()));
        }

        public bool Remove(ICacheKey key)
        {
            if (key == null || key.KeyString == string.Empty)
                return false;

            return this.redisClient.Remove(key.KeyString.ToUpper());
        }

        public bool RemoveFromUniqueList<T>(ICacheKey key, T ItemValue)
        {
            if (key == null || key.KeyString == string.Empty)
                return false;

            return this.redisClient.RemoveFromUniqueList(key.KeyString.ToUpper(), ItemValue);
        }

        public bool MakeTransaction(params RedisTransactionAction[] actions) {
            return this.redisClient.MakeTransaction(actions);
        }

        public RedisTransactionActionWithResult<T> CreateTxnGetAction<T>(eRedisTransactionActionType ActionType, ICacheKey Key)
        {
            //todo: consider adding this to a factory. Note: the constructor of this class could also use a factory. 
            //      (both the constructor and this method hardcode which .net redis library we are using)
            //      can then use service locator to state in (one place and only one place) which underlying .net library we are using.
            return new StackExchangeRedisClient.StackExchangeRedisTransactionActionWithResult<T>(ActionType, Key);
        }

        public bool GetAndRemove<T>(ICacheKey key, out T responseObj){   
            var getAction = CreateTxnGetAction<T>(eRedisTransactionActionType.Get, key);
            var removeAction = new RedisTransactionAction(eRedisTransactionActionType.Remove, key, null);
            bool outcome = MakeTransaction(getAction, removeAction);
            if (getAction.CastedValue != null)
                responseObj = getAction.CastedValue;
            else
                responseObj = default(T);
            return outcome;
        }
        
        public void Dispose()
        {
            if (redisClient == null) return;
            redisClient.Dispose();
        }
    }
}
