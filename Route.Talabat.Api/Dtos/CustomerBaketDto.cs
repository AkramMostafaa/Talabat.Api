using System.ComponentModel.DataAnnotations;

namespace Route.Talabat.Api.Dtos
{
    public class CustomerBaketDto
    {
        [Required]
        public string Id { get; set; }

        public List<BasketItemDto> Items { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ClientSecret { get; set; }

        public int? DeliveryMethodId { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
