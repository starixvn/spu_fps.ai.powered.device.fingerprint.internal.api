using Device.Fingerprint.Core;
using Device.Fingerprint.Core.Middleware;
using Device.Fingerprint.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Device.Fingerprint.Test.Core.Middleware
{
    /// <summary>
    /// Unit tests for JWTMiddleware class.
    /// Tests JWT token validation and HTTP request processing.
    /// </summary>
    public class JWTMiddlewareTests
    {
        private const string TestJWTPassword = "TestSecretKeyForJWTTokenValidation12345";
        private const string TestJWTAudience = "test-audience";

        private Mock<IAppSettings> CreateMockAppSettings()
        {
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(m => m.JWTPassword).Returns(TestJWTPassword);
            mockAppSettings.Setup(m => m.JWTAudience).Returns(TestJWTAudience);
            return mockAppSettings;
        }

        private string GenerateValidToken(int expirationMinutes = 30)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtService = new JWTService(CreateMockAppSettings().Object);
            var key = jwtService.keyJWT;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Role, "admin")
                }),
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateExpiredToken()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtService = new JWTService(CreateMockAppSettings().Object);
            var key = jwtService.keyJWT;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser")
                }),
                Expires = DateTime.UtcNow.AddMinutes(-30),
                NotBefore = DateTime.UtcNow.AddMinutes(-60),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private DefaultHttpContext CreateHttpContext(string authorizationHeader = null)
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            
            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                context.Request.Headers["Authorization"] = authorizationHeader;
            }

            return context;
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();

            // Act
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);

            // Assert
            middleware.Should().NotBeNull();
        }

        #endregion

        #region Invoke Tests - Valid Scenarios

        [Fact]
        public async Task Invoke_WithNoAuthorizationHeader_ShouldCallNextMiddleware()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Invoke_WithValidBearerToken_ShouldCallNextMiddleware()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var validToken = GenerateValidToken();
            var context = CreateHttpContext($"Bearer {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Invoke_WithValidTokenWithoutBearerPrefix_ShouldCallNextMiddleware()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var validToken = GenerateValidToken();
            var context = CreateHttpContext(validToken);

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        #endregion

        #region Invoke Tests - Invalid Scenarios

        [Fact]
        public async Task Invoke_WithExpiredToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var expiredToken = GenerateExpiredToken();
            var context = CreateHttpContext($"Bearer {expiredToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task Invoke_WithInvalidToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var context = CreateHttpContext("Bearer invalid.token.here");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task Invoke_WithMalformedToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var context = CreateHttpContext("Bearer notavalidtoken");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(next => next(context), Times.Never);
        }

        [Fact]
        public async Task Invoke_WithTokenSignedWithDifferentKey_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);

            // Create token with different key
            var tokenHandler = new JwtSecurityTokenHandler();
            var differentKey = Encoding.ASCII.GetBytes("DifferentSecretKeyThatDoesNotMatch123");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(differentKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = CreateHttpContext($"Bearer {tokenString}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(next => next(context), Times.Never);
        }

        #endregion

        #region Authorization Header Parsing Tests

        [Fact]
        public async Task Invoke_WithBearerPrefixAndSpaces_ShouldExtractToken()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var validToken = GenerateValidToken();
            var context = CreateHttpContext($"Bearer   {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithMultipleSpacesInHeader_ShouldExtractLastPart()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var validToken = GenerateValidToken();
            var context = CreateHttpContext($"Bearer Extra Parts {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithEmptyAuthorizationHeader_ShouldCallNextMiddleware()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var context = CreateHttpContext("");

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task Invoke_WithTokenAboutToExpire_ShouldStillBeValid()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            
            // Token expires in 1 second
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtService = new JWTService(mockAppSettings.Object);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }),
                Expires = DateTime.UtcNow.AddSeconds(1),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtService.keyJWT), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var context = CreateHttpContext($"Bearer {tokenString}");

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        #endregion

        #region Interface and Method Tests

        [Fact]
        public void JWTMiddleware_ShouldHaveInvokeMethod()
        {
            // Arrange
            var invokeMethod = typeof(JWTMiddleware).GetMethod("Invoke");

            // Assert
            invokeMethod.Should().NotBeNull();
            invokeMethod!.ReturnType.Should().Be(typeof(Task));
            invokeMethod.GetParameters().Should().HaveCount(1);
            invokeMethod.GetParameters()[0].ParameterType.Should().Be(typeof(HttpContext));
        }

        [Fact]
        public void JWTMiddleware_ShouldHavePrivateValidateTokenMethod()
        {
            // Arrange
            var validateTokenMethod = typeof(JWTMiddleware).GetMethod("ValidateToken", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            validateTokenMethod.Should().NotBeNull();
            validateTokenMethod!.ReturnType.Should().Be(typeof(bool));
        }

        [Fact]
        public void JWTMiddleware_ShouldHavePrivateCustomLifetimeValidatorMethod()
        {
            // Arrange
            var method = typeof(JWTMiddleware).GetMethod("CustomLifetimeValidator",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(bool));
            method.GetParameters().Should().HaveCount(4);
        }

        [Fact]
        public void JWTMiddleware_Constructor_ShouldHaveTwoParameters()
        {
            // Arrange
            var constructor = typeof(JWTMiddleware).GetConstructors()[0];

            // Assert
            constructor.GetParameters().Should().HaveCount(2);
            constructor.GetParameters()[0].ParameterType.Should().Be(typeof(RequestDelegate));
            constructor.GetParameters()[1].ParameterType.Should().Be(typeof(IAppSettings));
        }

        #endregion

        #region Response Handling Tests

        [Fact]
        public async Task Invoke_WithInvalidToken_ShouldClearResponseBeforeSettingStatus()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var context = CreateHttpContext("Bearer invalid.token");

            // Pre-set some response headers
            context.Response.Headers["X-Custom-Header"] = "SomeValue";

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            // Response should be cleared
            context.Response.Headers.Should().NotContainKey("X-Custom-Header");
        }

        [Fact]
        public async Task Invoke_MultipleCallsWithSameValidToken_ShouldAllSucceed()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockAppSettings = CreateMockAppSettings();
            var middleware = new JWTMiddleware(mockNext.Object, mockAppSettings.Object);
            var validToken = GenerateValidToken();

            // Act & Assert - Call 3 times
            for (int i = 0; i < 3; i++)
            {
                var context = CreateHttpContext($"Bearer {validToken}");
                await middleware.Invoke(context);
                mockNext.Verify(next => next(It.IsAny<HttpContext>()), Times.Exactly(i + 1));
            }
        }

        #endregion
    }
}
