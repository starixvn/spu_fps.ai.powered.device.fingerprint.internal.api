using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.Features.Subscriber.Services;
using Device.Fingerprint.Model;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Device.Fingerprint.Test.Controllers
{
    public class SubscriberControllerTests
    {
        private readonly Mock<ISubscriberServices> _mockSubscriberServices;
        private readonly SubscriberController _controller;

        public SubscriberControllerTests()
        {
            _mockSubscriberServices = new Mock<ISubscriberServices>();
            _controller = new SubscriberController(_mockSubscriberServices.Object);
        }

        [Fact]
        public void GetSubscriberInfo_WithValidSubscriberName_ShouldReturnSubscriber()
        {
            // Arrange
            var subscriberName = "TestSubscriber";
            var expectedSubscriber = new SubscriberModel
            {
                SubscriberId = 1,
                SubscriberName = subscriberName
            };

            _mockSubscriberServices
                .Setup(x => x.GetSubscriberByName(subscriberName))
                .Returns(expectedSubscriber);

            // Act
            var result = _controller.GetSubscriberInfo(subscriberName);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var subscriberModel = okResult.Value.Should().BeOfType<SubscriberModel>().Subject;
            
            subscriberModel.SubscriberId.Should().Be(expectedSubscriber.SubscriberId);
            subscriberModel.SubscriberName.Should().Be(expectedSubscriber.SubscriberName);
        }

        [Fact]
        public void GetSubscriberInfo_WithNonExistentSubscriber_ShouldReturnEmptyModel()
        {
            // Arrange
            var subscriberName = "NonExistentSubscriber";

            _mockSubscriberServices
                .Setup(x => x.GetSubscriberByName(subscriberName))
                .Returns((SubscriberModel?)null);

            // Act
            var result = _controller.GetSubscriberInfo(subscriberName);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var subscriberModel = okResult.Value.Should().BeOfType<SubscriberModel>().Subject;
            
            subscriberModel.SubscriberId.Should().Be(0);
            subscriberModel.SubscriberName.Should().BeNull();
        }

        [Fact]
        public void GetSubscriberInfo_WithEmptySubscriberName_ShouldCallService()
        {
            // Arrange
            var subscriberName = "";

            _mockSubscriberServices
                .Setup(x => x.GetSubscriberByName(subscriberName))
                .Returns((SubscriberModel?)null);

            // Act
            var result = _controller.GetSubscriberInfo(subscriberName);

            // Assert
            _mockSubscriberServices.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
            
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var subscriberModel = okResult.Value.Should().BeOfType<SubscriberModel>().Subject;
            subscriberModel.SubscriberId.Should().Be(0);
        }

        [Fact]
        public void GetSubscriberInfo_WithNullSubscriberName_ShouldCallService()
        {
            // Arrange
            string? subscriberName = null;

            _mockSubscriberServices
                .Setup(x => x.GetSubscriberByName(subscriberName!))
                .Returns((SubscriberModel?)null);

            // Act
            var result = _controller.GetSubscriberInfo(subscriberName!);

            // Assert
            _mockSubscriberServices.Verify(x => x.GetSubscriberByName(subscriberName!), Times.Once);
        }
    }
}