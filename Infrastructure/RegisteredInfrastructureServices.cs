//#define MOCKING

using Application.Common.Handlers;
using Infrastructure.Handlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Application.Common.Persistence;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Mocking;
using Domain.Entities;
using Application.Services;
using Infrastructure.Services;
using Application.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace Infrastructure
{
    public static class RegisteredInfrastructureServices
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ExceptionMiddleware>();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCache>();
            services.Configure<CachingOptions>(configuration.GetSection("MemoryCacheOptions"));

            services.AddSingleton<IExchangeRate, ExchangeRate>();
            services.Configure<ExchangeRatesConfigModel>(configuration.GetSection("ExchangeRateOptions"));

            services.AddAuthentication(configuration);
            services.AddPersistence(configuration);

            return services;
        }

        private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenHandler, Handlers.TokenHandler>();
            services.AddScoped<IPasswordHandler, PasswordHandler>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("Secrets:JwtToken").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });
            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {

#if MOCKING
            services.AddSingleton<MockDb>();
            services.AddScoped<IGenericRepository<User>, MockUserRepository>();
            services.AddScoped<IGenericRepository<Product>, MockProductRepository>();
            services.AddScoped<IGenericRepository<Order>, MockOrderRepository>();
            services.AddScoped<IUnitOfWork, MockUnitOfWork>();
#else
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
            });

            services.AddScoped<Func<ApplicationDbContext>>((provider) => provider.GetRequiredService<ApplicationDbContext>);
            services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
#endif
            return services;
        }
    }
}
