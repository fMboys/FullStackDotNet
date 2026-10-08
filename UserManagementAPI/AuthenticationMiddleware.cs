using System.Text.Json;

public sealed class TokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TokenValidationMiddleware> _logger;

    public TokenValidationMiddleware(
        RequestDelegate next,
        ILogger<TokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authorizationHeader =
            context.Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            await ReturnUnauthorizedAsync(
                context,
                "Authorization header is missing.");

            return;
        }

        if (!authorizationHeader.StartsWith("Bearer ",
            StringComparison.OrdinalIgnoreCase))
        {
            await ReturnUnauthorizedAsync(
                context,
                "Invalid authorization scheme.");

            return;
        }

        var token = authorizationHeader["Bearer ".Length..].Trim();

        if (!ValidateToken(token))
        {
            _logger.LogWarning(
                "Invalid token received for {Path}",
                context.Request.Path);

            await ReturnUnauthorizedAsync(
                context,
                "Invalid token.");

            return;
        }

        await _next(context);
    }

    private static bool ValidateToken(string token)
    {
        // Replace with real validation logic
        return token == "my-secret-token";
    }

    private static async Task ReturnUnauthorizedAsync(
        HttpContext context,
        string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new
        {
            error = message
        });
    }
}