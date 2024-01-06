using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Route.Talabat.Api.Dtos;
using Route.Talabat.Api.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Route.Talabat.Api.Controllers
{
   
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }

        [HttpGet] // GET : /api/Basket?id=
        public async Task<ActionResult<CustomerBasket>> GetBasket(string id)
        {

            var basket =  await _basketRepository.GetBasketAsync(id);
            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]  // POST : /api/basket
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBaketDto basketDto)
        {
            var mappedBasket = _mapper.Map<CustomerBaketDto, CustomerBasket>(basketDto);
            var createdOrUpdated= await _basketRepository.UpdateBasketAsync(mappedBasket); 
            if (createdOrUpdated is null)
                return BadRequest(new ApiResponse(400));
            return Ok(createdOrUpdated);

            
        }

        [HttpDelete] // Delete : /api/basket/
        public async Task DeleteBasket(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
