using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Device.Fingerprint.Core.Services;

namespace Device.Fingerprint.Core.Middleware
{
    public class JWTMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAppSettings appSettings;

        public JWTMiddleware(RequestDelegate next, IAppSettings appSettings)
        {
            this.next = next;
            this.appSettings = appSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            string token = null;
            if (authHeader != null)
            {
                var parts = authHeader.Split(' ');
                if (parts.Length > 0)
                {
                    token = parts[parts.Length - 1];
                }
            }

            if (token != null && !ValidateToken(token))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await next(context);
        }

        private bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate, TokenValidationParameters @param)
        {
            if (expires != null)
            {
                return expires > DateTimeOffset.UtcNow.DateTime;
            }
            return false;
        }

        private bool ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var JWTService = new JWTService(appSettings);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(JWTService.keyJWT),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    LifetimeValidator = CustomLifetimeValidator,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
