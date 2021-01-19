using System;
using System.Collections.Generic;

namespace DAL
{
    /*
        Config
        ------
        <appSettings>
            <add key="RedisPath" value="E:\Projects\BinanceTradeBot\PROJECT\BinanceTradeBot\DATA" />
            <add key="RedisEndpoint" value="127.0.0.1" />
            <add key="RedisPort" value="6379" />
        </appSettings>

        APP
        ---
        var bll = new BLL();
        var isRunning = bll.IsRedisRunning();
        if (!isRunning) {
            Console.WriteLine("Redis is NOT running, trying to launch it now...");
            Process.Start($"{ConfigSingleton.Instance.RedisPath}\\redis-server.exe", "--maxheap 300mb");
            Thread.Sleep(2000); // waiting for redis to be ready
        }

        bll.TestRedis();
        
        BLL
        ---
        IsRedisRunning:
        using (var redis = new RedisCacheManager())
            return redis.Connect(ConfigSingleton.Instance.RedisEndpoint, ConfigSingleton.Instance.RedisPort);            
        
        TestRedis:
        using (var redis = new RedisCacheManager()) {
            redis.Connect(ConfigSingleton.Instance.RedisEndpoint, ConfigSingleton.Instance.RedisPort);

            redis.Set("key1", "Hello World");
            var v = redis.Get<string>("key1");
            Console.WriteLine(v);
        } 
    */

    public class RedisCacheManager: IDisposable
    {
        private IRedisClient redisClient;

        public RedisCacheManager()
        {                 
            this.redisClient = new StackExchangeRedisClient();         
        }

        public bool Connect(string RedisEndpoint, int RedisPort)
        {
            try
            {
                Console.WriteLine("Connecting to Redis...");
                this.redisClient.Connect(RedisEndpoint, RedisPort);
                Console.WriteLine("Connected!");
                return true;
            }
            catch {
                return false;
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
