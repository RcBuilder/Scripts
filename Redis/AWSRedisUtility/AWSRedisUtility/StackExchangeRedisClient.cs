using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using StackExchange.Redis;

namespace AWSRedisUtility
{
    internal class StackExchangeRedisClient : IRedisClient
    {
        private ConnectionMultiplexer connector = null;
        private IDatabase dataBase = null;
        private IServer server = null;

        public StackExchangeRedisClient(string EndPoint, int Port)
        {
            var strEndPoint = string.Format("{0}:{1}", EndPoint, Port);

            var config = ConfigurationOptions.Parse(strEndPoint);
            config.AllowAdmin = true;

            this.connector = ConnectionMultiplexer.Connect(config);
            this.dataBase = this.connector.GetDatabase();
            this.server = connector.GetServer(this.connector.GetEndPoints().First());
        }

        public bool Remove(string Key)
        {
            if (string.IsNullOrEmpty(Key))
                return false;
            return this.dataBase.KeyDelete(Key);
        }

        public bool Set<T>(string Key, T Value, DateTime? ExpiryDate = null)
        {
            if (string.IsNullOrEmpty(Key))
                return false;

            var result = this.dataBase.StringSet(Key, JsonConvert.SerializeObject(Value));
            if (ExpiryDate.HasValue)
                this.dataBase.KeyExpire(Key, ExpiryDate);

            return result;
        }

        public T Get<T>(string Key)
        {
            if (string.IsNullOrEmpty(Key))
                return default(T);

            var value = this.dataBase.StringGet(Key);
            if (value.IsNull)
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }

        public IEnumerable<T> GetAll<T>()
        {
            var keys = this.server.Keys().Select(x => (string)x);
            return GetMultiple<T>(keys);
        }

        public IEnumerable<T> GetMultiple<T>(IEnumerable<string> Keys)
        {
            var redisKeys = Keys.Select(x => (RedisKey)x).ToArray();
            var entries = this.dataBase.StringGet(redisKeys);

            if (entries == null)
                return null;

            T entry;
            var result = new List<T>();

            foreach (var e in entries)
            {
                var isT = TryDeserializeObject<T>(e.ToString(), out entry);
                if (!isT) continue;
                result.Add(entry);
            }

            return result;
        }

        public IEnumerable<string> GetAllKeys()
        {
            return this.server.Keys().Select(x => (string)x);
        }

        public IEnumerable<string> SearchKeys(string Pattern)
        {
            if (string.IsNullOrEmpty(Pattern))
                return null;

            return this.server.Keys(pattern: string.Format("*{0}*", Pattern)).Select(k => k.ToString());
        }

        public long AddToList<T>(string Key, T Value)
        {
            if (string.IsNullOrEmpty(Key))
                return -1;

            return this.dataBase.ListRightPush(Key, JsonConvert.SerializeObject(Value)) - 1;
        }

        public long AddToList<T>(string Key, IEnumerable<T> Values)
        {
            if (string.IsNullOrEmpty(Key))
                return -1;

            var redisValues = Values.Select(x => (RedisValue)JsonConvert.SerializeObject(x));
            return this.dataBase.ListRightPush(Key, redisValues.ToArray()) - 1;
        }

        public T GetFromList<T>(string Key, long ItemIndex)
        {
            if (string.IsNullOrEmpty(Key))
                return default(T);

            return JsonConvert.DeserializeObject<T>(this.dataBase.ListGetByIndex(Key, ItemIndex));
        }

        public IEnumerable<T> GetTopXFromList<T>(string Key, int Quantity)
        {
            if (string.IsNullOrEmpty(Key) || Quantity < 1)
                return null;

            var redisResults = this.dataBase.ListRange(Key, 0, Quantity - 1);

            var results = new List<T>();
            foreach (var redisValue in redisResults)
            {
                if (redisValue.IsNull)
                    results.Add(default(T));
                results.Add(JsonConvert.DeserializeObject<T>(redisValue));
            }

            return results;
        }

        public bool RemoveFromList<T>(string Key, T ItemValue)
        {
            if (string.IsNullOrEmpty(Key))
                return false;

            // create an actual serialized redis value and remove ALL of its instances from the currect list
            var redisValue = (RedisValue)JsonConvert.SerializeObject(ItemValue);
            return this.dataBase.ListRemove(Key, redisValue) > 0;
        }

        public bool AddToUniqueList<T>(string Key, T Value)
        {
            if (string.IsNullOrEmpty(Key))
                return false;

            // score (Ticks) - in order to prevent the default chronological order and make the insertion time the valid order 
            // note! if two items inserted with the very same score - the secondary order will be the chronological value
            return this.dataBase.SortedSetAdd(Key, JsonConvert.SerializeObject(Value), DateTime.Now.Ticks);
        }

        public bool AddToUniqueList<T>(string Key, IEnumerable<T> Values)
        {
            if (string.IsNullOrEmpty(Key))
                return false;

            var sortedSetValues = Values.Select(x => new SortedSetEntry((RedisValue)JsonConvert.SerializeObject(x), DateTime.Now.Ticks));
            return this.dataBase.SortedSetAdd(Key, sortedSetValues.ToArray()) > 0;
        }

        public IEnumerable<T> GetTopXFromUniqueList<T>(string Key, int Quantity)
        {
            if (string.IsNullOrEmpty(Key) || Quantity < 1)
                return null;

            // SortedSetRangeByRank - get top x ordered by insertion index
            // SortedSetRangeByScore - get top x ordered by score values
            var redisResults = this.dataBase.SortedSetRangeByRank(Key, 0, Quantity, Order.Ascending);

            var results = new List<T>();
            foreach (var redisValue in redisResults)
            {
                if (redisValue.IsNull)
                    results.Add(default(T));
                results.Add(JsonConvert.DeserializeObject<T>(redisValue));
            }

            return results;
        }

        public bool MakeTransaction(params RedisTransactionAction[] actions)
        {
            var trans = this.dataBase.CreateTransaction();

            /* 
                structure:
                trans.AddCondition(Condition)
                
                e.g: 
                // KEY_A is mandatory - if not exists in the cache - transaction will fail 
                trans.AddCondition(Condition.KeyExists("KEY_A")); 
            */

            foreach (var action in actions)
            {
                switch (action.ActionType)
                {
                    case eRedisTransactionActionType.Set:
                        var entity = Convert.ChangeType(action.Value, action.ValueType);
                        var hasExpiry = action.Properties.ContainsKey("EXPIRY");
                        trans.StringSetAsync(action.Key, JsonConvert.SerializeObject(entity)).ContinueWith(_ =>
                        {
                            if (hasExpiry)
                                trans.KeyExpireAsync(action.Key, ((DateTime)action.Properties["EXPIRY"]));
                        });
                        break;
                    case eRedisTransactionActionType.AddToUniqueList:
                        trans.ListRightPushAsync(action.Key, JsonConvert.SerializeObject(action.Value));
                        break;
                }
            }

            return trans.Execute();
        }

        public void FlushALL()
        {
            this.server.FlushDatabase();
        }

        private bool TryDeserializeObject<T>(string Json, out T Value)
        {
            try
            {
                Value = JsonConvert.DeserializeObject<T>(Json);
                return true;
            }
            catch
            {
                Value = default(T);
                return false;
            }
        }

        public void Dispose()
        {
            this.connector.Dispose();
        }
    }
}
