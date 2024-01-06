using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Route.Talabat.Api.Errors;
using Stripe;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggergate;
using Talabat.Core.Service.Contract;

namespace Route.Talabat.Api.Controllers
{

    [Authorize]
    public class PaymentsController :BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;
        const string _whSecret = "whsec_5e326627755d8976cdd0caf404608526119776d7d67715ee54f9b77dddda6fe4";

        public PaymentsController(IPaymentService paymentService,ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [ProducesResponseType(typeof(CustomerBasket),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpPost("{basketId}")] // POST : /api/Payments
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntents(string basketId)
         {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket is null)
                return BadRequest(new ApiResponse(400,"An Error With Your Basket"));

            return Ok(basket);

        }
        [HttpPost("webhook")] //POST : /api/Payments/webhook

        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], _whSecret);
                var paymentIntent =(PaymentIntent) stripeEvent.Data.Object;
                Order order;
                // Handle the event
                switch (stripeEvent.Type)
                {
                    case Events.PaymentIntentSucceeded:
                        order = await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id,true);
                        _logger.LogInformation("Payment is Succeeded ", paymentIntent.Id);
                        break;
                    case Events.PaymentIntentPaymentFailed:
                        order= await _paymentService.UpdatePaymentIntentToSucceededOrFailed(paymentIntent.Id, false);
                        _logger.LogInformation("Payment is Failed :(", paymentIntent.Id);

                        break;


                }       

                return Ok();
       

        }


    }
}
