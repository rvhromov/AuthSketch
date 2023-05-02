using AuthSketch.Exceptions;
using AuthSketch.Extensions;
using AuthSketch.Options;
using System.Text.Json;

namespace AuthSketch.Middlewares;

public sealed class ExceptionMiddleware : IMiddleware
{
    private readonly TfaOptions _tfaoptions;

    public ExceptionMiddleware(IConfiguration configuration) =>
        _tfaoptions = configuration.GetOptions<TfaOptions>(nameof(TfaOptions));

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		try
		{
			await next(context);
		}
		catch (Exception exception)
		{
            var statusCode = GetStatusCode(exception);

            context.Response.StatusCode = statusCode;
            context.Response.Headers.Add("content-type", "application/json");

            var json = JsonSerializer.Serialize(new { ErrorCode = statusCode, exception.Message });
            await context.Response.WriteAsync(json);
        }
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
}
