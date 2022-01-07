
using Entities;
using System.Collections.Generic;

namespace DAL
{
    public interface ISystemBrokersDAL {       
        IEnumerable<BrokerData> GetBrokers();         
        BrokerData GetBroker(string name);        
        bool SaveBroker(BrokerData brokerData);   
        bool DeleteBroker(string name);

        IEnumerable<QueryData> GetQueries();
        int CreateQuery(QueryData queryData);
        bool DeleteQuery(int id);
    }
}
