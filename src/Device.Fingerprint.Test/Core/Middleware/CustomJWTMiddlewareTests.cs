using Device.Fingerprint.Core.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Device.Fingerprint.Test.Core.Middleware
{
    /// <summary>
    /// Unit tests for CustomJWTMiddleware class.
    /// Tests middleware behavior for JWT validation in different scenarios.
    /// </summary>
    public class CustomJWTMiddlewareTests
    {
        private const string TestJWTPassword = "TestSecretKey123456789012345678901234567890";
        private const string InvalidJWTPassword = "InvalidSecretKey1234567890123456789012345";

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();

            // Act
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);

            // Assert
            middleware.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithJWTDisabled_ShouldInitialize()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();

            // Act
            var middleware = new CustomJWTMiddleware(mockNext.Object, false, TestJWTPassword);

            // Assert
            middleware.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithEmptyPassword_ShouldInitialize()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();

            // Act
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, string.Empty);

            // Assert
            middleware.Should().NotBeNull();
        }

        #endregion

        #region Invoke - JWT Enabled Tests

        [Fact]
        public async Task Invoke_JWTEnabled_WithValidToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var validToken = GenerateValidToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {validToken}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Invoke_JWTEnabled_WithNoToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Never);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Invoke_JWTEnabled_WithInvalidToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            context.Request.Headers["Authorization"] = "Bearer invalid.token.here";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Never);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Invoke_JWTEnabled_WithTokenSignedByDifferentKey_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var tokenWithWrongKey = GenerateValidToken(InvalidJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {tokenWithWrongKey}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Never);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Invoke_JWTEnabled_WithEmptyAuthHeader_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            context.Request.Headers["Authorization"] = string.Empty;

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Never);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        #endregion

        #region Invoke - JWT Disabled Tests

        [Fact]
        public async Task Invoke_JWTDisabled_WithNoToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, false, TestJWTPassword);
            var context = CreateHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Invoke_JWTDisabled_WithValidToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, false, TestJWTPassword);
            var context = CreateHttpContext();
            var validToken = GenerateValidToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {validToken}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
            context.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Invoke_JWTDisabled_WithInvalidToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, false, TestJWTPassword);
            var context = CreateHttpContext();
            context.Request.Headers["Authorization"] = "Bearer invalid.token.here";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Never);
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        #endregion

        #region Authorization Header Parsing Tests

        [Fact]
        public async Task Invoke_WithBearerPrefix_ShouldParseTokenCorrectly()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var validToken = GenerateValidToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {validToken}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithOnlyToken_ShouldParseTokenCorrectly()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var validToken = GenerateValidToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = validToken;

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithMultipleSpacesInHeader_ShouldParseLastPart()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var validToken = GenerateValidToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer Extra {validToken}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        #endregion

        #region Token Validation Edge Cases

        [Fact]
        public async Task Invoke_WithMalformedToken_ShouldReturn401()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            context.Request.Headers["Authorization"] = "Bearer not.a.valid.jwt.token";

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Invoke_WithExpiredToken_ShouldStillPass()
        {
            // Arrange - Middleware has ValidateLifetime = false
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var expiredToken = GenerateExpiredToken(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {expiredToken}";

            // Act
            await middleware.Invoke(context);

            // Assert - Should pass because ValidateLifetime is false
            mockNext.Verify(next => next(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WithTokenWithoutExpiration_ShouldPass()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, true, TestJWTPassword);
            var context = CreateHttpContext();
            var tokenWithoutExp = GenerateTokenWithoutExpiration(TestJWTPassword);
            context.Request.Headers["Authorization"] = $"Bearer {tokenWithoutExp}";

            // Act
            await middleware.Invoke(context);

            // Assert
            mockNext.Verify(next => next(context), Times.Once);
        }

        #endregion

        #region Method Signature Tests

        [Fact]
        public void CustomJWTMiddleware_ShouldHaveInvokeMethod()
        {
            // Arrange
            var method = typeof(CustomJWTMiddleware).GetMethod("Invoke");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
            method.IsPublic.Should().BeTrue();
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(HttpContext));
        }

        [Fact]
        public void CustomJWTMiddleware_ShouldHavePrivateValidateTokenMethod()
        {
            // Arrange
            var method = typeof(CustomJWTMiddleware).GetMethod("ValidateToken", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(bool));
            method.IsPrivate.Should().BeTrue();
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(string));
        }

        [Fact]
        public void CustomJWTMiddleware_Constructor_ShouldHaveCorrectSignature()
        {
            // Arrange
            var constructor = typeof(CustomJWTMiddleware).GetConstructors().FirstOrDefault();

            // Assert
            constructor.Should().NotBeNull();
            constructor!.GetParameters().Should().HaveCount(3);
            constructor.GetParameters()[0].ParameterType.Should().Be(typeof(RequestDelegate));
            constructor.GetParameters()[1].ParameterType.Should().Be(typeof(bool));
            constructor.GetParameters()[2].ParameterType.Should().Be(typeof(string));
        }

        #endregion

        #region Field Tests

        [Fact]
        public void CustomJWTMiddleware_ShouldHavePrivateNextField()
        {
            // Arrange
            var field = typeof(CustomJWTMiddleware).GetField("next", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            field.Should().NotBeNull();
            field!.FieldType.Should().Be(typeof(RequestDelegate));
        }

        [Fact]
        public void CustomJWTMiddleware_ShouldHavePrivateJWTPasswordField()
        {
            // Arrange
            var field = typeof(CustomJWTMiddleware).GetField("JWTPassword", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            field.Should().NotBeNull();
            field!.FieldType.Should().Be(typeof(string));
        }

        [Fact]
        public void CustomJWTMiddleware_ShouldHavePrivateJWTEnableField()
        {
            // Arrange
            var field = typeof(CustomJWTMiddleware).GetField("JWTEnable", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Assert
            field.Should().NotBeNull();
            field!.FieldType.Should().Be(typeof(bool));
        }

        #endregion

        #region Helper Methods

        private static HttpContext CreateHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new System.IO.MemoryStream();
            return context;
        }

        private static string GenerateValidToken(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Role, "TestRole")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateExpiredToken(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser")
                }),
                NotBefore = DateTime.UtcNow.AddHours(-2), // Valid from 2 hours ago
                Expires = DateTime.UtcNow.AddHours(-1), // Expired 1 hour ago
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateTokenWithoutExpiration(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "TestUser")
                }),
                // No Expires set
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }
}
