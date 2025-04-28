
using CodeCart.API.Errors;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace CodeCart.API.Middlewares;

public class ExceptionMiddleware(IHostEnvironment environment, ILogger<ExceptionMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		try
		{
			await next(context);
		}
        catch (Exception ex)
        {

            logger.LogError(ex, ex.Message);


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = environment.IsDevelopment() ?
                new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, ex.Message, ex.StackTrace?.ToString()) :
                new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }
    }
}
