using System;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Serilog;
using Anemoi.BuildingBlock.Application.Errors;

namespace Anemoi.BuildingBlock.Infrastructure.GeneralMiddlewares;

public sealed class ExceptionMiddleware(RequestDelegate next, IHostEnvironment env, ILogger logger)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            logger.Error("Error while executing a request: {Error}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = StatusCodes.Status500InternalServerError;
        if (exception is ValidationException validationException)
        {
            logger.Error("Validation Error stackTrace: {@StackTrace}", validationException.StackTrace);
            response.StatusCode = StatusCodes.Status400BadRequest;
            await response.WriteAsJsonAsync(new { validationException.Errors });
            return;
        }

        var isDevelopment = env.IsDevelopment();
        if (isDevelopment)
        {
            await response.WriteAsync(JsonConvert.SerializeObject(exception));
            return;
        }

        await response.WriteAsync(JsonConvert.SerializeObject(GlobalErrors.UnexpectedError()));
    }
}