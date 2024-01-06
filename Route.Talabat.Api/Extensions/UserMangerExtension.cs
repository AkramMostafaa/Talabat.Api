using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Route.Talabat.Api.Extensions
{
    public static class UserMangerExtension
    {
        public async static Task<AppUser> FindUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user =await userManager.Users.Include(U => U.Address).SingleOrDefaultAsync(U=>U.Email==email);

            return user;


        }
    }
}
