using CodeCart.API.Errors;
using CodeCart.Core.Services.Contracts.SecurityModule;
using System.Net;
using System.Text.Json;

namespace CodeCart.API.Middlewares;

public class JwtBlacklistMiddleware(RequestDelegate next, ITokenBlacklistService tokenBlacklistService,
     ILogger<JwtBlacklistMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                var isBlacklisted = await tokenBlacklistService.IsTokenBlacklistedAsync(token);
                if (isBlacklisted)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Response.ContentType = "application/json";

                    var response = new ApiResponse(401, "Token has been revoked");
                    var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    await context.Response.WriteAsync(jsonResponse);
                    return;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        await next(context);
    }
}