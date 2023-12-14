using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Handlers
{
    public sealed class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger _logger;

        public ExceptionMiddleware(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<ExceptionMiddleware>();
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
                _logger.LogError($"Exception occured: {ex.Message}");
            }
        }

        public static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            //TODO update exception handling
            var statusCode = 500;
            var response = new
            {
                status = statusCode,
                code = "System Error",
                detail = exception.Message,
                errors = exception.Data.Values
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}

