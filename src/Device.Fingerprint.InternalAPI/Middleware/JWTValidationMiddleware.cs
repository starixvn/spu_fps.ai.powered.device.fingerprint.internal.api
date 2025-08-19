using Device.Fingerprint.InternalAPI.Model;
using Device.Fingerprint.Core;
using Microsoft.AspNetCore.Authorization;

namespace Device.Fingerprint.InternalAPI.Middleware
{
    public class JWTValidationMiddleware(RequestDelegate next, IAppSettings appSettings)
    {
        private readonly RequestDelegate next = next;
        private readonly IAppSettings appSettings = appSettings;

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await next(context);
                return;
            }

            // Add null check for context.Request.Path.Value
            var pathValue = context.Request.Path.Value;
            if (pathValue != null && !pathValue.Contains("generatetoken/audience"))
            {
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

                if (string.IsNullOrWhiteSpace(token) || !ValidateToken(context, token))
                {
                    context.Response.Clear();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    return;
                }
            }
            else if (pathValue == null)
            {
                // If path is null, return 401 for security
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next(context);
        }

        private bool ValidateToken(HttpContext context, string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var jwtSetting = appSettings.Configuration.GetSection("AppConfig:JWT:APIConfig").Get<IEnumerable<APIConfigModel>>();
                var path = context.Request.Path.Value;

                // Handle null jwtSetting to prevent ArgumentNullException
                // Also handle null path
                if (jwtSetting != null && path != null)
                {
                    var matchedApi = jwtSetting.FirstOrDefault(item => path.StartsWith(item.API));

                    if (matchedApi != null && matchedApi.Token == token)
                    {
                        return true;
                    }
                }
            }

            return false;

        }
    }
}
