namespace IndigoApi2.Middleware;

public class ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private const string ApiKeyName = "ApiKey";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? "";
        
        if (path.StartsWith("/scalar", StringComparison.OrdinalIgnoreCase) || 
            path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }
        
        // 1. Check if the header exists
        if (!context.Request.Headers.TryGetValue(ApiKeyName, out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key was not provided.");
            return;
        }

        // 2. Get the real key from config
        var apiKey = configuration.GetValue<string>("Authentication:ApiKey");

        // 3. Compare them
        if (!apiKey!.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Unauthorized client.");
            return;
        }

        // 4. Key is valid! Move to the next step in the pipeline
        await next(context);
    }
}