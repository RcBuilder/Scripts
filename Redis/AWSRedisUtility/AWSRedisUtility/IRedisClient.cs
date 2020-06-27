using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSRedisUtility
{
    public interface IRedisClient : IDisposable
    {
        bool Remove(string Key);
        bool Set<T>(string Key, T Value, DateTime? ExpiryDate = null);
        T Get<T>(string Key);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetMultiple<T>(IEnumerable<string> Keys);
        IEnumerable<string> GetAllKeys();
        IEnumerable<string> SearchKeys(string Pattern);
        long AddToList<T>(string Key, T Value);  // return the item index
        long AddToList<T>(string Key, IEnumerable<T> Values);  // return the item index
        T GetFromList<T>(string Key, long ItemIndex);
        IEnumerable<T> GetTopXFromList<T>(string Key, int Quantity);
        bool RemoveFromList<T>(string Key, T ItemValue);
        bool AddToUniqueList<T>(string Key, T Value);
        bool AddToUniqueList<T>(string Key, IEnumerable<T> Values);
        IEnumerable<T> GetTopXFromUniqueList<T>(string Key, int Quantity);
        bool MakeTransaction(params RedisTransactionAction[] actions);
        void FlushALL();
    }

    public class RedisTransactionAction
    {
        public eRedisTransactionActionType ActionType { set; get; }
        public string Key { set; get; }
        public object Value { set; get; }
        public Type ValueType { private set; get; }
        public Dictionary<string, object> Properties { set; get; }

        public RedisTransactionAction(eRedisTransactionActionType ActionType, string Key, object Value)
        {
            this.ActionType = ActionType;
            this.Key = Key;
            this.Value = Value;
            this.ValueType = Value.GetType();
            this.Properties = new Dictionary<string, object>();
        }
    }

    public enum eRedisTransactionActionType { NULL, Set, AddToUniqueList }
}
