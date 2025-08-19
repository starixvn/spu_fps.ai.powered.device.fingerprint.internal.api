using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.Features.Account.Model;
using Device.Fingerprint.Features.Account.Services;
using Fanex.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Device.Fingerprint.Test.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountServices> _mockAccountServices;
        private readonly Mock<ILogger> _mockLogger;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAccountServices = new Mock<IAccountServices>();
            _mockLogger = new Mock<ILogger>();
            _controller = new AccountController(_mockAccountServices.Object, _mockLogger.Object);
        }

        [Fact]
        public void SearchAccount_WithValidParam_ShouldReturnAccountResult()
        {
            // Arrange
            var param = new SearchAccountParamModel
            {
                UserName = "test@example.com",
                SubscriberID = 1
            };

            var expectedResult = new SearchAccountResultModel
            {
                TotalItem = 1,
                Items = new List<SearchAccountItemModel>
                {
                    new SearchAccountItemModel
                    {
                        SubscriberID = 1,
                        UserName = "test@example.com",
                        CTSCustID = 100
                    }
                }
            };

            _mockAccountServices
                .Setup(x => x.SearchAccount(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.SearchAccount(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountResult = okResult.Value.Should().BeOfType<SearchAccountResultModel>().Subject;
            
            accountResult.TotalItem.Should().Be(1);
            accountResult.Items.Should().HaveCount(1);
            accountResult.Items?.First().UserName.Should().Be("test@example.com");
        }

        [Fact]
        public void SearchAccount_WhenExceptionThrown_ShouldReturnEmptyResultAndLogError()
        {
            // Arrange
            var param = new SearchAccountParamModel();
            var exceptionMessage = "Database error";

            _mockAccountServices
                .Setup(x => x.SearchAccount(param))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.SearchAccount(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountResult = okResult.Value.Should().BeOfType<SearchAccountResultModel>().Subject;

            accountResult.TotalItem.Should().Be(0);
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Search Account info exception"))), Times.Once);
        }

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_WithValidParam_ShouldReturnAccountInfo()
        {
            // Arrange
            var param = new GetAssAccountInfoByCustSubscriberParamModel
            {
                CTSCustID = 100,
                SubscriberID = 1
            };

            var expectedAccountInfo = new GetAssAccountInfoByCustSubscriberModel
            {
                AssociatedAccounts = 5,
                AssociatedDevices = 3,
                FirstLogin = DateTime.Now.AddDays(-30),
                FirstDevice = "Device123"
            };

            _mockAccountServices
                .Setup(x => x.GetAssAccountInfoByCustSubscriber(param))
                .Returns(expectedAccountInfo);

            // Act
            var result = _controller.GetAssAccountInfoByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountInfo = okResult.Value.Should().BeOfType<GetAssAccountInfoByCustSubscriberModel>().Subject;
            
            accountInfo.AssociatedAccounts.Should().Be(5);
            accountInfo.AssociatedDevices.Should().Be(3);
            accountInfo.FirstDevice.Should().Be("Device123");
        }

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_WhenExceptionThrown_ShouldReturnEmptyModelAndLogError()
        {
            // Arrange
            var param = new GetAssAccountInfoByCustSubscriberParamModel();
            var exceptionMessage = "Service error";

            _mockAccountServices
                .Setup(x => x.GetAssAccountInfoByCustSubscriber(param))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.GetAssAccountInfoByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountInfo = okResult.Value.Should().BeOfType<GetAssAccountInfoByCustSubscriberModel>().Subject;
            
            accountInfo.AssociatedAccounts.Should().Be(0);
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Ass Account Info by Cust Subscriber exception"))), Times.Once);
        }

        [Fact]
        public void GetAssAccountsByCustSubscriber_WithValidParam_ShouldReturnAccountsResult()
        {
            // Arrange
            var param = new GetAssAccountsByCustSubscriberParamModel
            {
                CTSCustID = 100,
                SubscriberID = 1
            };

            var expectedResult = new GetAssAccountsByCustSubscriberResultModel
            {
                Items = new List<GetAssAccountsByCustSubscriberItemModel>
                {
                    new GetAssAccountsByCustSubscriberItemModel
                    {
                        CTSCustID = 100,
                        UserName = "user1@test.com",
                        DeviceInCommon = 2
                    },
                    new GetAssAccountsByCustSubscriberItemModel
                    {
                        CTSCustID = 101,
                        UserName = "user2@test.com",
                        DeviceInCommon = 1
                    }
                }
            };

            _mockAccountServices
                .Setup(x => x.GetAssAccountsByCustSubscriber(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.GetAssAccountsByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountsResult = okResult.Value.Should().BeOfType<GetAssAccountsByCustSubscriberResultModel>().Subject;
            
            accountsResult.Items.Should().HaveCount(2);
            accountsResult.Items.First().UserName.Should().Be("user1@test.com");
        }

        [Fact]
        public void GetAssAccountsByCustSubscriber_WhenExceptionThrown_ShouldReturnEmptyResultAndLogError()
        {
            // Arrange
            var param = new GetAssAccountsByCustSubscriberParamModel();
            var exceptionMessage = "Service error";

            _mockAccountServices
                .Setup(x => x.GetAssAccountsByCustSubscriber(param))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.GetAssAccountsByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var accountsResult = okResult.Value.Should().BeOfType<GetAssAccountsByCustSubscriberResultModel>().Subject;
            
            accountsResult.Items.Should().BeNullOrEmpty();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Ass Accounts by Cust Subscriber exception"))), Times.Once);
        }

        [Fact]
        public void GetAssDevicesByCustSubscriber_WithValidParam_ShouldReturnDevicesResult()
        {
            // Arrange
            var param = new GetAssDevicesByCustSubscriberParamModel
            {
                CTSCustID = 100,
                SubscriberID = 1
            };

            var expectedResult = new GetAssDevicesByCustSubscriberResultModel
            {
                Items = new List<GetAssDevicesByCustSubscriberItemModel>
                {
                    new GetAssDevicesByCustSubscriberItemModel
                    {
                        DeviceID = 1,
                        DeviceCode = "DEVICE123",
                        FirstSeenDate = DateTime.Now.AddDays(-10),
                        AssociatedAccounts = 3
                    }
                }
            };

            _mockAccountServices
                .Setup(x => x.GetAssDevicesByCustSubscriber(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.GetAssDevicesByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var devicesResult = okResult.Value.Should().BeOfType<GetAssDevicesByCustSubscriberResultModel>().Subject;
            
            devicesResult.Items.Should().HaveCount(1);
            devicesResult.Items.First().DeviceCode.Should().Be("DEVICE123");
        }

        [Fact]
        public void GetAssDevicesByCustSubscriber_WhenExceptionThrown_ShouldReturnEmptyResultAndLogError()
        {
            // Arrange
            var param = new GetAssDevicesByCustSubscriberParamModel();
            var exceptionMessage = "Service error";

            _mockAccountServices
                .Setup(x => x.GetAssDevicesByCustSubscriber(param))
                .Throws(new Exception(exceptionMessage));

            // Act
            var result = _controller.GetAssDevicesByCustSubscriber(param);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var devicesResult = okResult.Value.Should().BeOfType<GetAssDevicesByCustSubscriberResultModel>().Subject;
            
            devicesResult.Items.Should().BeNullOrEmpty();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Ass Devices by Cust Subscriber exception"))), Times.Once);
        }
    }
}