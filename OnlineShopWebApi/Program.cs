using Application;
using Infrastructure;
using Infrastructure.Handlers;
using OnlineShopWebApi.Extensions;

namespace OnlineShopWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Logging.ClearProviders();
            //builder.Logging.AddConsole();

            builder.Services
                .AddUI()
                .AddApplicationLayer()
                .AddInfrastructureLayer(builder.Configuration);

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}