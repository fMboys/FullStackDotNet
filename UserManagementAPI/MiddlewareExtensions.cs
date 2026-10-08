public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

public static class RequestResponseLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestResponseLogging(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}

public static class TokenValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseTokenValidation(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<TokenValidationMiddleware>();
    }
}