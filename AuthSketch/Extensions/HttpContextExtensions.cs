namespace AuthSketch.Extensions;

public static class HttpContextExtensions
{
    public static string GetIpAddress(this HttpContext httpContext)
    {
        var ip = httpContext.Connection.RemoteIpAddress;

        if (ip is null)
        {
            return default;
        }

        return ip.MapToIPv4().ToString();
    }

    public static void SetCookie(this HttpContext httpContext, string key, string value)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        httpContext.Response.Cookies.Append(key, value, options);
    }

    public static string GetCookie(this HttpContext httpContext, string key)
    {
        var isRetrieved = httpContext.Request.Cookies.TryGetValue(key, out var cookie);

        return isRetrieved ? cookie : default;
    }
}
