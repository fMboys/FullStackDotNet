using System.Diagnostics;
using System.Text;

public sealed class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

    public RequestResponseLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        // Read Request Body
        context.Request.EnableBuffering();

        string requestBody = string.Empty;

        if (context.Request.ContentLength > 0)
        {
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true);

            requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        // Capture Response Body
        var originalResponseBodyStream = context.Response.Body;

        await using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await _next(context);

            stopwatch.Stop();

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body)
                .ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(
                """
                HTTP Request/Response

                TraceId: {TraceId}
                Method: {Method}
                Path: {Path}
                QueryString: {QueryString}

                Request Body:
                {RequestBody}

                Response Status: {StatusCode}

                Response Body:
                {ResponseBody}

                Elapsed: {ElapsedMs} ms
                """,
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                requestBody,
                context.Response.StatusCode,
                responseBody,
                stopwatch.ElapsedMilliseconds);

            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
        }
        finally
        {
            context.Response.Body = originalResponseBodyStream;
        }
    }
}

