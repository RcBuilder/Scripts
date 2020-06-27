using CommonEntities.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSCommon.Redis
{
    public class RedisTransactionAction
    {
        public eRedisTransactionActionType ActionType { set; get; }
        public ICacheKey Key { set; get; }
        public object Value { set; get; }
        public Type ValueType { protected set; get; }
        public Dictionary<string, object> Properties { set; get; }

        protected RedisTransactionAction(eRedisTransactionActionType ActionType, ICacheKey Key)
        {
            this.ActionType = ActionType;
            if (Key == null)
                throw new ArgumentNullException("parameter key cannot be null");
            this.Key = Key;
            this.Properties = new Dictionary<string, object>();
        }

        public RedisTransactionAction(eRedisTransactionActionType ActionType, ICacheKey Key, object Value)
            : this(ActionType, Key)
        {
            this.Value = Value;
            if (Value != null)
                this.ValueType = Value.GetType();
        }

        public virtual void PostProcess(object Result) { }
    }

    public abstract class RedisTransactionActionWithResult<T> : RedisTransactionAction
    {
        public T CastedValue { get; protected set; }

        public RedisTransactionActionWithResult(eRedisTransactionActionType ActionType, ICacheKey Key)
            : base(ActionType, Key)
        {
            this.ValueType = typeof(T);
        }

        public abstract override void PostProcess(object Result);
    }

    public enum eRedisTransactionActionType { NULL, Set, AddToUniqueList, Remove, Get, RemoveFromUniqueList }
}
