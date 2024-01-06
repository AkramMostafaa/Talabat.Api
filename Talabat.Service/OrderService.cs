using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggergate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications.OrderSpecs;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basketRepo,
            IUnitOfWork unitOfWork,IPaymentService paymentService

           
            )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int deliveryIdMethod, Address shippingAddress)
        {
            // 1.Get Basket From Baskets Repo

            var basket =  await _basketRepo.GetBasketAsync(basketId);

            // 2.Get Selected Items At Basket From Products Repo
            var orderItems = new List<OrderItem>();
            var productRepo = _unitOfWork.Repository<Product>();

            if (basket?.Items?.Count() > 0)
            {
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);

                    var productItemOrder = new ProductItemOrder(item.Id,product.Name,product.PictureUrl);

                    var orderItem = new OrderItem(productItemOrder,product.Price,item.Quantity);

                    orderItems.Add(orderItem);
                }
            }

            // 3.Calculate Subtotal

            var subTotal = orderItems.Sum(O => O.Price * O.Quantity);

            // 4.Get Delivery Method From DeliveryMethod Repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryIdMethod);
            var orderRepo =  _unitOfWork.Repository<Order>();

            var orderSpec = new OrderWithPaymentIntentSpecifications(basket.PaymentIntentId);


            var existingOrder = await orderRepo.GetEntityWithSpecAsync(orderSpec);

            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            // 5.Create Order 
            var order = new Order(buyerEmail,shippingAddress,deliveryMethod,orderItems,subTotal,basket.PaymentIntentId);

            await orderRepo.AddAsync(order);

            // 6.Save To Database [TODO]
           var result = await _unitOfWork.CompleteAsync();
            if (result <= 0) return null;
            return order;


        }

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        => _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        public  Task<Order?> GetOrderByIdForUserAsync(int orderId, string buyerEmail)
        {
            var orderRepo= _unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications(orderId, buyerEmail);
            var order = orderRepo.GetEntityWithSpecAsync(spec);

            return  order; 
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
                var orderRepo=_unitOfWork.Repository<Order>();
            var spec = new OrderSpecifications( buyerEmail);
            var orders = await orderRepo.GetAllWithSpecAsync(spec);
            return orders;
        }
    }
}
