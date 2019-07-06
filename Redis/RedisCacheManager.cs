using Amazon;
using Amazon.ElastiCache;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AWSCommon.Redis
{
    public class RedisCacheManager: IDisposable
    {
        private IRedisClient redisClient;

        public RedisCacheManager()
        {
            try
            {
                //this.redisClient = new ServiceStackRedisClient(ConfigurationManager.AppSettings["RedisEndpoint"], Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]));
                this.redisClient = new StackExchangeRedisClient(ConfigurationManager.AppSettings["RedisEndpoint"], Convert.ToInt32(ConfigurationManager.AppSettings["RedisPort"]));
            }
            catch
            {
               // TODO Log
               // "Can't load redis Client"
            }
        }

        public bool Set<T>(string key, T value, DateTime? ExpiryDate = null)
        {
            if (key == string.Empty || value == null)
                return false;

            var result = this.redisClient.Set<T>(key, value, ExpiryDate);
            return result;
        }

        public T Get<T>(string key)
        {
            if (key == string.Empty)
                return default(T);

            var value = this.redisClient.Get<T>(key);
            if (value == null)
                return default(T);

            return value;
        }

        public IEnumerable<T> GetMultiple<T>(IEnumerable<string> keys)
        {
            return this.redisClient.GetMultiple<T>(keys);
        }

        public IEnumerable<T> GetALL<T>() {
            return this.redisClient.GetAll<T>();
        }

        public void Dispose()
        {
            redisClient.Dispose();
        }
    }
}
