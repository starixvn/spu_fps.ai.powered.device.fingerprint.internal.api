using Device.Fingerprint.Core;
using Device.Fingerprint.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Xunit;

namespace Device.Fingerprint.Test.Core.Services
{
    public class JWTServiceTests
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private const string TestJwtPassword = "TestSecretKey123456789012345678901234567890";
        private const string TestApiUrl = "https://test-api.example.com";

        public JWTServiceTests()
        {
            _mockAppSettings = new Mock<IAppSettings>();
            _mockAppSettings.Setup(x => x.JWTPassword).Returns(TestJwtPassword);
            _mockAppSettings.Setup(x => x.ApiUrl).Returns(TestApiUrl);
        }

        [Fact]
        public void Constructor_ShouldInitializeKeyJWT_WhenCalled()
        {
            // Act
            var service = new JWTService(_mockAppSettings.Object);

            // Assert
            service.keyJWT.Should().NotBeNull();
            service.keyJWT.Should().BeEquivalentTo(Encoding.ASCII.GetBytes(TestJwtPassword));
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidToken_WhenCalled()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);

            // Act
            var token = service.GenerateJwtToken();

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Should().NotBeNull();
            jwtToken.Issuer.Should().Be(TestApiUrl);
            jwtToken.Claims.Should().Contain(c => c.Type == "id" && c.Value == TestApiUrl);
        }

        [Fact]
        public void GenerateJwtToken_ShouldHaveValidExpiration_WhenCalled()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);

            // Act
            var token = service.GenerateJwtToken();

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Verify the token has a valid structure with expiration
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().NotBe(default(DateTime));
        }

        [Fact]
        public void GenerateJWT_ShouldReturnValidToken_WithCustomExpiration()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);
            var expiredTime = DateTimeOffset.UtcNow.AddHours(2);
            var audience = "test-audience";

            // Act
            var token = service.GenerateJWT(expiredTime, audience);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Verify the token has a valid structure with expiration
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().NotBe(default(DateTime));
        }

        [Fact]
        public void GenerateJWT_ShouldUseAudienceInKeyGeneration_WhenCalled()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(30);
            var audience1 = "audience-one";
            var audience2 = "audience-two";

            // Act
            var token1 = service.GenerateJWT(expiredTime, audience1);
            var token2 = service.GenerateJWT(expiredTime, audience2);

            // Assert
            token1.Should().NotBeNullOrEmpty();
            token2.Should().NotBeNullOrEmpty();
            token1.Should().NotBe(token2, "different audiences should generate different tokens");
        }

        [Fact]
        public void GenerateJWT_ShouldHandleNullAudience_WithoutException()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(15);

            // Act
            Action act = () => service.GenerateJWT(expiredTime, null);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void GenerateJWT_ShouldHandleEmptyAudience_WithoutException()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(15);

            // Act
            var token = service.GenerateJWT(expiredTime, string.Empty);

            // Assert
            token.Should().NotBeNullOrEmpty();
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            jwtToken.Should().NotBeNull();
        }

        [Fact]
        public void GenerateJWT_ShouldThrowException_WhenExpirationIsInPast()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);
            var expiredTime = DateTimeOffset.UtcNow.AddMinutes(-10); // Past time

            // Act & Assert
            Action act = () => service.GenerateJWT(expiredTime, "test");
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GenerateJwtToken_ShouldUseHmacSha256Algorithm_WhenCalled()
        {
            // Arrange
            var service = new JWTService(_mockAppSettings.Object);

            // Act
            var token = service.GenerateJwtToken();

            // Assert
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            jwtToken.Header.Alg.Should().Be("HS256");
        }
    }
}
