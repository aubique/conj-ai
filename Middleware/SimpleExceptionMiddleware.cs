using System;
using System.Text.Json;
using conj_ai.Models;

namespace conj_ai.Middleware;

public class SimpleExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SimpleExceptionMiddleware> _logger;

    public SimpleExceptionMiddleware(RequestDelegate next, ILogger<SimpleExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            var (statusCode, message) = ex switch
            {
                ConfigurationException => (500, ex.Message),
                UnauthorizedAiException => (502, ex.Message),
                _ => (500, "Internal server error")
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = new { error = message, statusCode };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}