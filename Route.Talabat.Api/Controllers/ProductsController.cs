using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Route.Talabat.Api.Dtos;
using Route.Talabat.Api.Errors;
using Route.Talabat.Api.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Service.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Route.Talabat.Api.Controllers
{
  
    public class ProductsController :BaseApiController
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;


        public ProductsController(
            IProductService productService
            
            ,IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }


        //[Authorize]
        [CachedAttribute(600)] // Action Filter 
        [HttpGet]       // GET : /Api/Products

        
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams specParams)
        {

            var products = await _productService.GetProductsAsync(specParams);

            var data = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(products);

            var count = await _productService.GetCountAsync(specParams);

            return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex,specParams.PageSize,data,count));
        }


        [ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        [HttpGet("{id}")] // GET : Api/Product/1
        public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null)
                return NotFound(new ApiResponse(404)) ;

            return Ok(_mapper.Map<Product,ProductToReturnDto>(product));
        }


        [HttpGet("brands")] // GET : /api/product/brands
        [ProducesResponseType(typeof(ProductBrand), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var brands = await _productService.GetBrandsAsync();
            return Ok(brands);
            
        }


        [ProducesResponseType(typeof(ProductCategory),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [HttpGet("categories")]
        public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()  // GET : /api/product/categories
        {
            var categories = await _productService.GetCategoriesAsync();
            return Ok(categories);
        }
    }
}
