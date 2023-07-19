using System;
using System.Threading.Tasks;

namespace Tookan
{
    public interface ITookanManager
    {
        Task<PickupDeliveryTaskResponse> CreatePickupDeliveryTask(PickupDeliveryTaskRequest Request);        
    }
}
