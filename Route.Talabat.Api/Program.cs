using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Route.Talabat.Api.Errors;
using Route.Talabat.Api.Extensions;
using Route.Talabat.Api.Helpers;
using Route.Talabat.Api.Middlewares;
using StackExchange.Redis;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;
using Talabat.Repository.Identity.DataSeed;

namespace Route.Talabat.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);



            #region Configure Service
            // Add services to the container.

            builder.Services.AddControllers();
                //.AddNewtonsoftJson(options =>
            //{
            //    options.SerializerSettings.ReferenceLoopHandling= ReferenceLoopHandling.Ignore;
            //    });

            builder.Services.AddSwaggerServices();

            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>();
          

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddSingleton<IConnectionMultiplexer>(
                (serviceProvider) => 
                {
                    var connection = builder.Configuration.GetConnectionString("Redis");
                    return ConnectionMultiplexer.Connect(connection);
                } );

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            { 
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddCors(options=>
            {
                options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader().AllowAnyMethod().WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });


            #endregion



            var app = builder.Build();

           using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var _dbContext = service.GetRequiredService<StoreContext>();
            var _identityDbContext = service.GetRequiredService<AppIdentityDbContext>();

            var loggerFactory = service.GetRequiredService<ILoggerFactory>();


            try
            {
                await _dbContext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(_dbContext);
                await _identityDbContext.Database.MigrateAsync();

                var _userManger = service.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(_userManger);



            }
            catch (Exception ex)
            {
                var _logger = loggerFactory.CreateLogger<Program>();

                _logger.LogError(ex,"An Error Has Been Occured During Apply The Migration");

            }
            // Configure the HTTP request pipeline.
            #region Configure Kestrel Middlewares

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {

            }
            app.UseSwaggerMiddleware();

            app.UseStatusCodePagesWithRedirects("/errors/{0}"); 

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");

            app.MapControllers();
            app.UseAuthentication();
            app.UseAuthorization();

            #endregion

            app.Run();

        }
    }
}