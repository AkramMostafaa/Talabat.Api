using AutoMapper;
using Route.Talabat.Api.Dtos;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggergate;

using UserAddress = Talabat.Core.Entities.Identity.Address;
using OrderAddress = Talabat.Core.Entities.Order_Aggergate.Address;

namespace Route.Talabat.Api.Helpers
{
    public class MappingProfile:Profile
    {

        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.Category, O => O.MapFrom(s => s.Category.Name))
                //.ForMember(d => d.PictureUrl, O => O.MapFrom(s => $"{"https://localhost:7214"}/{s.PictureUrl}"));
                .ForMember(d => d.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

            CreateMap<CustomerBaketDto, CustomerBasket>();
            CreateMap<BasketItemDto, BasketItem>();

            //CreateMap<AddressDto, OrderAddress>();
            CreateMap<UserAddress, AddressDto>().ReverseMap();
            CreateMap<OrderAddress, AddressDto>().ReverseMap();
            


          

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(O => O.DeliveryMethod, O => O.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(O => O.DeliveryMethodCost, O => O.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.ProductId))
                  .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                  .ForMember(d=>d.PictureUrl,o=>o.MapFrom(s=>s.Product.PictureUrl))
                    .ForMember(d=>d.PictureUrl ,o=>o.MapFrom<OrderItemPictureUrlResolver>())  ;

          
        }

    }
}
