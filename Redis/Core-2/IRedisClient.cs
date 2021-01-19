using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    internal interface IRedisClient : IDisposable
    {
        void Connect(string EndPoint, int Port);
        bool Remove(string Key);
        bool Set<T>(string Key, T Value, DateTime? ExpiryDate = null);
        T Get<T>(string Key);
        string GetAsJson(string Key);
        IEnumerable<T> GetAll<T>();
        IEnumerable<T> GetMultiple<T>(IEnumerable<string> Keys);
        IEnumerable<string> GetMultipleAsJson(IEnumerable<string> Keys);
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
        bool RemoveFromUniqueList<T>(string Key, T ItemValue);
        bool RemoveFromUniqueList<T>(string Key, IEnumerable<T> ItemsValue);        

        void FlushALL();
    }
}
