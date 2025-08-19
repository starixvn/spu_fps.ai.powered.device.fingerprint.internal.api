using Device.Fingerprint.InternalAPI.Middleware;
using Device.Fingerprint.InternalAPI.Model;
using Device.Fingerprint.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Device.Fingerprint.Test.Middleware
{
    public class JWTValidationMiddlewareTests
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IConfigurationSection> _mockConfigSection;

        public JWTValidationMiddlewareTests()
        {
            _mockAppSettings = new Mock<IAppSettings>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfigSection = new Mock<IConfigurationSection>();
            
            _mockAppSettings.Setup(x => x.Configuration).Returns(_mockConfiguration.Object);
        }

        private void SetupApiConfigMock(IEnumerable<APIConfigModel>? apiConfigs)
        {
            // Create a mock configuration builder to handle the Get<T>() extension method
            var configBuilder = new ConfigurationBuilder();
            
            if (apiConfigs != null)
            {
                // Create a dictionary to simulate the configuration values
                var configDict = new Dictionary<string, string>();
                var configList = apiConfigs.ToList();
                
                for (int i = 0; i < configList.Count; i++)
                {
                    configDict[$"AppConfig:JWT:APIConfig:{i}:API"] = configList[i].API ?? "";
                    configDict[$"AppConfig:JWT:APIConfig:{i}:Token"] = configList[i].Token ?? "";
                }
                
                configBuilder.AddInMemoryCollection(configDict);
            }
            else
            {
                // For null apiConfigs, create an empty configuration
                configBuilder.AddInMemoryCollection(new Dictionary<string, string>());
            }

            var configuration = configBuilder.Build();
            var section = configuration.GetSection("AppConfig:JWT:APIConfig");
            
            // Setup the section to return our configured section
            _mockConfiguration.Setup(x => x.GetSection("AppConfig:JWT:APIConfig"))
                             .Returns(section);
        }

        [Fact]
        public async Task Invoke_WithAllowAnonymousAttribute_ShouldCallNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            var nextCalled = false;

            // Setup endpoint with AllowAnonymous attribute
            var endpoint = new Endpoint(
                requestDelegate: null,
                metadata: new EndpointMetadataCollection(new AllowAnonymousAttribute()),
                displayName: "Test endpoint"
            );
            context.SetEndpoint(endpoint);

            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithGenerateTokenPath_ShouldCallNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/generatetoken/audience";
            var nextCalled = false;

            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithoutAuthorizationHeader_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Response.Body = new MemoryStream();

            // Setup empty API config list
            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithInvalidToken_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/GetTransactionInfo";
            context.Request.Headers["Authorization"] = "Bearer invalid_token";
            context.Response.Body = new MemoryStream();

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/GetTransactionInfo",
                    Token = "valid_token"
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithValidToken_ShouldCallNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer valid_token";
            var nextCalled = false;

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/test",
                    Token = "valid_token"
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithPartialPathMatch_ShouldCallNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test/subpath";
            context.Request.Headers["Authorization"] = "Bearer valid_token";
            var nextCalled = false;

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/test",
                    Token = "valid_token"
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithNoMatchingAPI_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/different";
            context.Request.Headers["Authorization"] = "Bearer some_token";
            context.Response.Body = new MemoryStream();

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/test",
                    Token = "valid_token"
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithNullConfiguration_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer some_token";
            context.Response.Body = new MemoryStream();

            // Setup configuration to return null - this should create empty configuration
            SetupApiConfigMock(null);

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithEmptyAuthorizationHeader_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = ""; // Empty authorization header
            context.Response.Body = new MemoryStream();

            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithMalformedAuthorizationHeader_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "InvalidFormat"; // No "Bearer " prefix
            context.Response.Body = new MemoryStream();

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/test",
                    Token = "valid_token" // Different from the malformed header
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithNullPath_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            // Don't set the path - this will test the null path scenario
            context.Response.Body = new MemoryStream();

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            // The middleware should now handle null paths gracefully and return 401
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithBearerTokenButNoSpace_ShouldExtractCorrectToken()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer"; // Only "Bearer" without space and token
            context.Response.Body = new MemoryStream();

            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithMultipleSpacesInAuthHeader_ShouldExtractLastPart()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer extra space valid_token";
            var nextCalled = false;

            var apiConfigs = new List<APIConfigModel>
            {
                new APIConfigModel
                {
                    API = "/api/test",
                    Token = "valid_token" // Should match the last part after split
                }
            };

            SetupApiConfigMock(apiConfigs);

            RequestDelegate next = (HttpContext ctx) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            nextCalled.Should().BeTrue();
        }

        [Fact]
        public async Task Invoke_WithWhoamiPath_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/whoami";
            context.Response.Body = new MemoryStream();

            // Note: Based on the actual middleware code, /whoami is NOT exempted
            // Only /generatetoken/audience is exempted
            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithCaseInsensitivePath_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/GENERATETOKEN/AUDIENCE"; // Test case sensitivity
            context.Response.Body = new MemoryStream();

            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert - This will return 401 because the middleware is case-sensitive
            // The middleware checks for exact case match
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithEmptyApiConfigList_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer some_token";
            context.Response.Body = new MemoryStream();

            // Setup with empty API config list
            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }

        [Fact]
        public async Task Invoke_WithWhitespaceOnlyToken_ShouldReturn401()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/api/test";
            context.Request.Headers["Authorization"] = "Bearer   "; // Whitespace only token
            context.Response.Body = new MemoryStream();

            SetupApiConfigMock(new List<APIConfigModel>());

            RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;

            var middleware = new JWTValidationMiddleware(next, _mockAppSettings.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(401);
        }
    }
}