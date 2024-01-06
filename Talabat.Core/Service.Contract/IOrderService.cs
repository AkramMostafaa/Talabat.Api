using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggergate;

namespace Talabat.Core.Service.Contract
{
    public interface IOrderService
    {
        public Task<Order?> CreateOrderAsync(string buyerEmail,string basketId,int deliveryIdMethod,Address shippingAddress);

        public Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);

        public Task<Order?> GetOrderByIdForUserAsync(int orderId,string buyerEmail);
        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
    }
}
