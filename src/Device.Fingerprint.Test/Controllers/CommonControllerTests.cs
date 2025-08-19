using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.Core;
using Device.Fingerprint.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;

namespace Device.Fingerprint.Test.Controllers
{
    public class CommonControllerTests
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly CommonController _controller;

        public CommonControllerTests()
        {
            _mockAppSettings = new Mock<IAppSettings>();
            _controller = new CommonController(_mockAppSettings.Object);
        }

        [Fact]
        public void Index_ShouldReturnWhoamiModel_WithCorrectValues()
        {
            // Arrange
            var expectedEnvironment = "Test";
            var expectedLocalIP = "127.0.0.1";
            
            _mockAppSettings.Setup(x => x.Environment).Returns(expectedEnvironment);

            // Setup HttpContext with connection features
            var httpContext = new DefaultHttpContext();
            var connectionFeature = new Mock<IHttpConnectionFeature>();
            connectionFeature.Setup(x => x.LocalIpAddress).Returns(IPAddress.Parse(expectedLocalIP));
            httpContext.Features.Set(connectionFeature.Object);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be(expectedEnvironment);
            result.ServerName.Should().Be(Environment.MachineName);
            result.ServerTime.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
            result.LocalIP.Should().Be(expectedLocalIP);
        }

        [Fact]
        public void Index_ShouldHandleNullLocalIpAddress()
        {
            // Arrange
            var expectedEnvironment = "Test";
            
            _mockAppSettings.Setup(x => x.Environment).Returns(expectedEnvironment);

            // Setup HttpContext with null local IP
            var httpContext = new DefaultHttpContext();
            var connectionFeature = new Mock<IHttpConnectionFeature>();
            connectionFeature.Setup(x => x.LocalIpAddress).Returns((IPAddress?)null);
            httpContext.Features.Set(connectionFeature.Object);
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be(expectedEnvironment);
            result.ServerName.Should().Be(Environment.MachineName);
            result.LocalIP.Should().BeNull();
        }

        [Fact]
        public void Index_ShouldHandleNullHttpConnectionFeature()
        {
            // Arrange
            var expectedEnvironment = "Production";
            
            _mockAppSettings.Setup(x => x.Environment).Returns(expectedEnvironment);

            // Setup HttpContext without connection feature
            var httpContext = new DefaultHttpContext();
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be(expectedEnvironment);
            result.ServerName.Should().Be(Environment.MachineName);
            result.LocalIP.Should().BeNull();
        }
    }
}