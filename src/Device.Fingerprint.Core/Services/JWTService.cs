using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Device.Fingerprint.Core.Services
{
    public class JWTService
    {
        private readonly IAppSettings appSettings;
        // SonarQube S2357, S3887: keyJWT needs to be public for external JWT validation usage
#pragma warning disable S2357, S3887
        public readonly byte[] keyJWT;
#pragma warning restore S2357, S3887

        public JWTService(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
            keyJWT = Encoding.ASCII.GetBytes(appSettings.JWTPassword);
        }
        public string GenerateJwtToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", appSettings.ApiUrl) }),
                Expires = DateTimeOffset.UtcNow.AddMinutes(1).DateTime,
                Issuer = appSettings.ApiUrl,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyJWT), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateJWT(DateTimeOffset expiredTime, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.JWTPassword + "@" + audience));
            var tokenDescriptor = new SecurityTokenDescriptor
            {                
                Expires = expiredTime.DateTime,               
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
