using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity.DataSeed
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> _userManger)
        {
            if (_userManger.Users.Count() == 0)
            {
                var user = new AppUser()
                {
                    DisplayName="Akram Mostafa",
                    Email="Akrammostafa114@gmail.com",
                    UserName="Akram.Mostafa",
                    PhoneNumber="01067916344"
                };
                await _userManger.CreateAsync(user,"Pa$$w0rd");

            }
        }
    }
}
