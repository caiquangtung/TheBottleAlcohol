using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Alcohol.Middleware;

public class TokenLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenLoggingMiddleware> _logger;

    public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            // _logger.LogInformation("Authorization Header: {AuthHeader}", authHeader);

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                // _logger.LogInformation("Access Token: {Token}", token);

                // Log token parts
                var tokenParts = token.Split('.');
                // _logger.LogInformation("Token Parts Count: {Count}", tokenParts.Length);
                
                if (tokenParts.Length != 3)
                {
                    // _logger.LogError("Invalid token format. Expected 3 parts but got {Count}", tokenParts.Length);
                }
                else
                {
                    // _logger.LogInformation("Token Header: {Header}", tokenParts[0]);
                    // _logger.LogInformation("Token Payload: {Payload}", tokenParts[1]);
                    // _logger.LogInformation("Token Signature: {Signature}", tokenParts[2]);
                }
            }
            else
            {
                _logger.LogWarning("No Bearer token found in Authorization header");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing token in middleware");
        }

        await _next(context);
    }
} 