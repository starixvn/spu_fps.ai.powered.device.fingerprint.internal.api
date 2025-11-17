using Device.Fingerprint.InternalAPI.Model;
using Device.Fingerprint.Core;
using Microsoft.AspNetCore.Authorization;

namespace Device.Fingerprint.InternalAPI.Middleware
{
    public class JWTValidationMiddleware(RequestDelegate next, IAppSettings appSettings)
    {
        private readonly RequestDelegate next = next;
        private readonly IAppSettings appSettings = appSettings;
        private const string SkipValidationPath = "generatetoken/audience";

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata?.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                await next(context);
                return;
            }

            var pathValue = context.Request.Path.Value;
            if (pathValue != null && pathValue.Contains(SkipValidationPath))
            {
                await next(context);
                return;
            }

            if (pathValue == null || !IsAuthorized(context))
            {
                SetUnauthorizedResponse(context);
                return;
            }

            await next(context);
        }

        private bool IsAuthorized(HttpContext context)
        {
            var token = ExtractToken(context);
            return !string.IsNullOrWhiteSpace(token) && ValidateToken(context, token);
        }

        private static string ExtractToken(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader == null)
            {
                return null;
            }

            var parts = authHeader.Split(' ');
            return parts.Length > 0 ? parts[parts.Length - 1] : null;
        }

        private static void SetUnauthorizedResponse(HttpContext context)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
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
