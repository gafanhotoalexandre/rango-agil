using System.Net;

namespace RangoAgil.API.EndpointFilters;

public class LogNotFoundEndpointFilter(ILogger<LogNotFoundEndpointFilter> logger) : IEndpointFilter
{
    public readonly ILogger<LogNotFoundEndpointFilter> _logger = logger;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var result = await next(context);
        var actualResult = (result is INestedHttpResult result1) ? result1.Result : (IResult)result;

        if (actualResult is IStatusCodeHttpResult { StatusCode: (int)HttpStatusCode.NotFound })
        {
            _logger.LogInformation($"Resource {context.HttpContext.Request.Path} was not found.");
        }

        return result;
    }
}
