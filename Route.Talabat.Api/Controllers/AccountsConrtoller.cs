using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Route.Talabat.Api.Dtos;
using Route.Talabat.Api.Errors;
using Route.Talabat.Api.Extensions;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Service.Contract;

namespace Route.Talabat.Api.Controllers
{
    
    public class AccountsController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManger;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public  AccountsController(UserManager<AppUser> userManger,SignInManager<AppUser> signInManager,IAuthService authService,IMapper mapper)
        {
            _userManger = userManger;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")] // POST : /api/Account/login
        public async Task<ActionResult<UserDto>> Login(LoginDto model) 
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            if(user is null )
                return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user,model.Password,false);

            if (result.Succeeded is false)
                return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto() 
            {
                DisplayName=user.DisplayName,
                Email=user.Email,
                Token=await _authService.CreateTokenAsync(user,_userManger)

            });

         }

        [HttpPost("register")] // POST : /api/Account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest(new ApiValidationErrorResponse() { Errors=new string[] {"this email is already in use!!" } });

            var user = new AppUser() 
            {
                DisplayName=model.DisplayName,
                Email=model.Email,    
                UserName = model.Email.Split("@")[0],
                PhoneNumber=model.PhoneNumber
            };

            var result = await _userManger.CreateAsync(user,model.Password);
            if (result.Succeeded is false)
                return BadRequest(new ApiResponse(400));
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManger)
            }) ;
        }
        [Authorize]
        [HttpGet] // GET : /api/Accounts/
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email =User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManger.FindByEmailAsync(email);

            return Ok(new UserDto()
            {

                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManger)
            });
        }
        [Authorize]
        [ProducesResponseType(typeof(AddressDto),StatusCodes.Status203NonAuthoritative)]
        [HttpGet("address")]
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManger.FindUserWithAddressAsync(User);
            var address = _mapper.Map<AddressDto>(user.Address);
            return Ok(address);
        }
        [Authorize]
        [ProducesResponseType(typeof(AddressDto), StatusCodes.Status203NonAuthoritative)]

        [HttpPut("address")]  // PUT : /api/Accounts/address
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto updatedAddress)
        {
            var address = _mapper.Map<AddressDto, Address>(updatedAddress);

            var user = await _userManger.FindUserWithAddressAsync(User);
            address.Id = user.Address.Id;
            user.Address=address;

            var result = await _userManger.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400)); 
            return Ok(updatedAddress);
        }

        [HttpGet("emailExists")] //GET : /api/Accounts/email?email=Akrammostafa114@gmail.com
        public async Task<ActionResult<bool>> CheckEmailExists([FromQuery]string email)
        {
            return await _userManger.FindByEmailAsync(email) is not null;

        }
    }
}
