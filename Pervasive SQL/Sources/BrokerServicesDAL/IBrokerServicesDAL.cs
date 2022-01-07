
using Entities;
using System.Collections.Generic;

namespace DAL
{
    public interface IBrokerServicesDAL
    {
        IEnumerable<BrokerData> GetBrokers();
        bool RegisterWebHook(WebHookData data);
        bool DeleteWebHook(int id, string brokerName);
        bool UpdateWebHook(WebHookData data);
        bool UpdateWebHookLastExecutionTime(WebHookData data);
        IEnumerable<WebHookData> GetBrokerWebHooks(string brokerName);
        IEnumerable<WebHookData> GetServiceWebHooks(string brokerName, string serviceName);
    }
}
