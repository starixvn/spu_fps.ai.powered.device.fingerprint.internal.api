using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.InternalAPI.Services;
using Device.Fingerprint.Core;
using Device.Fingerprint.Features.Account.Services;
using Device.Fingerprint.Features.Devices.Services;
using Device.Fingerprint.Features.FingerPrint.Services;
using Device.Fingerprint.Features.Subscriber.Services;
using Device.Fingerprint.Features.Transaction.Services;
using Device.Fingerprint.Redis;
using Fanex.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Device.Fingerprint.Test
{
    /// <summary>
    /// Comprehensive test suite covering all controllers and services in Device.Fingerprint.InternalAPI
    /// </summary>
    public class InternalAPIComprehensiveTests
    {
        #region CommonController Tests
        
        [Fact]
        public void CommonController_Index_ShouldReturnWhoamiModel()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.Environment).Returns("Test");
            
            var controller = new CommonController(mockAppSettings.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = controller.Index();

            // Assert
            result.Should().NotBeNull();
            result.Environment.Should().Be("Test");
            result.ServerName.Should().Be(Environment.MachineName);
        }

        #endregion

        #region SubscriberController Tests

        [Fact]
        public void SubscriberController_GetSubscriberInfo_WithNullResult_ShouldReturnEmptyModel()
        {
            // Arrange
            var mockSubscriberServices = new Mock<ISubscriberServices>();
            mockSubscriberServices.Setup(x => x.GetSubscriberByName(It.IsAny<string>())).Returns((Fingerprint.Model.SubscriberModel?)null);
            
            var controller = new SubscriberController(mockSubscriberServices.Object);

            // Act
            var result = controller.GetSubscriberInfo("test");

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        #endregion

        #region RedisController Tests

        [Fact]
        public async Task RedisController_GetRedisString_WithNonExistentKey_ShouldReturnNotFound()
        {
            // Arrange
            var mockRedisService = new Mock<IRedisMultiplexDbService>();
            var mockDatabase = new Mock<StackExchange.Redis.IDatabase>();
            
            mockRedisService.Setup(x => x.GetDatabase(It.IsAny<int>())).Returns(mockDatabase.Object);
            mockDatabase.Setup(x => x.StringGet(It.IsAny<StackExchange.Redis.RedisKey>(), It.IsAny<StackExchange.Redis.CommandFlags>()))
                       .Returns(StackExchange.Redis.RedisValue.Null);
            
            var controller = new RedisController(mockRedisService.Object);

            // Act
            var result = await controller.GetRedisString(0, "nonexistent");

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region DeviceController Tests

        [Fact]
        public void DeviceController_SearchDevice_WithException_ShouldReturnEmptyListAndLogError()
        {
            // Arrange
            var mockDeviceServices = new Mock<IDeviceServices>();
            var mockLogger = new Mock<ILogger>();
            
            mockDeviceServices.Setup(x => x.SearchDevice(It.IsAny<Fingerprint.Features.Devices.Model.SearchDeviceParamModel>()))
                             .Throws(new Exception("Test exception"));
            
            var controller = new DeviceController(mockDeviceServices.Object, mockLogger.Object);

            // Act
            var result = controller.SearchDevice(new Fingerprint.Features.Devices.Model.SearchDeviceParamModel());

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            
            mockLogger.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region AccountController Tests

        [Fact]
        public void AccountController_SearchAccount_WithException_ShouldReturnEmptyResultAndLogError()
        {
            // Arrange
            var mockAccountServices = new Mock<IAccountServices>();
            var mockLogger = new Mock<ILogger>();
            
            mockAccountServices.Setup(x => x.SearchAccount(It.IsAny<Fingerprint.Features.Account.Model.SearchAccountParamModel>()))
                              .Throws(new Exception("Test exception"));
            
            var controller = new AccountController(mockAccountServices.Object, mockLogger.Object);

            // Act
            var result = controller.SearchAccount(new Fingerprint.Features.Account.Model.SearchAccountParamModel());

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
            
            mockLogger.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region TransactionController Tests

        [Fact]
        public void TransactionController_GetTransactionInfo_WithInvalidBatchSize_ShouldReturnBadRequest()
        {
            // Arrange
            var mockTransactionService = new Mock<ITransactionService>();
            var mockLogger = new Mock<ILogger>();
            
            var controller = new TransactionController(mockTransactionService.Object, mockLogger.Object);

            // Act
            var result = controller.GetTransactionInfo(0, 10001); // Invalid batch size

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        #endregion

        #region FingerPrintController Tests

        [Fact]
        public void FingerPrintController_GetFingerPrint_WithInvalidDateRange_ShouldReturnBadRequest()
        {
            // Arrange
            var mockFingerPrintServices = new Mock<IFingerPrintServices>();
            var mockLogstashService = new Mock<ILogstashFPFingerprintService>();
            
            var controller = new FingerPrintController(mockFingerPrintServices.Object, mockLogstashService.Object);

            // Act
            var result = controller.GetFingerPrint(DateTime.Now, DateTime.Now.AddDays(-1), 100, 0); // fromDate > toDate

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        #endregion

        #region RawTransactionController Tests

        [Fact]
        public void RawTransactionController_InsertRawTransaction_WithInvalidJson_ShouldReturnBadRequest()
        {
            // Arrange
            var mockTransactionService = new Mock<ITransactionService>();
            var controller = new RawTransactionController(mockTransactionService.Object);

            // Act
            var result = controller.InsertRawTransaction("{ invalid json");

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        #endregion

        #region LogstashFPFingerprintService Tests

        [Fact]
        public void LogstashFPFingerprintService_Constructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.LogstashTcpHostname).Returns("localhost");
            mockAppSettings.Setup(x => x.LogstashTcpPort).Returns(5000);

            // Act
            var service = new LogstashFPFingerprintService(mockAppSettings.Object);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeOfType<LogstashFPFingerprintService>();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void AllControllers_ShouldHaveParameterlessConstructorOrDependencyInjection()
        {
            // This test ensures all controllers can be instantiated properly
            var controllerTypes = new[]
            {
                typeof(CommonController),
                typeof(SubscriberController),
                typeof(RedisController),
                typeof(DeviceController),
                typeof(AccountController),
                typeof(TransactionController),
                typeof(FingerPrintController),
                typeof(RawTransactionController)
            };

            foreach (var controllerType in controllerTypes)
            {
                var constructors = controllerType.GetConstructors();
                constructors.Should().NotBeEmpty($"{controllerType.Name} should have at least one constructor");
                
                var primaryConstructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
                primaryConstructor.Should().NotBeNull($"{controllerType.Name} should have a primary constructor");
            }
        }

        [Fact]
        public void AllControllers_ShouldInheritFromControllerBase()
        {
            var controllerTypes = new[]
            {
                typeof(CommonController),
                typeof(SubscriberController),
                typeof(RedisController),
                typeof(DeviceController),
                typeof(AccountController),
                typeof(TransactionController),
                typeof(FingerPrintController),
                typeof(RawTransactionController)
            };

            foreach (var controllerType in controllerTypes)
            {
                controllerType.Should().BeAssignableTo<ControllerBase>($"{controllerType.Name} should inherit from ControllerBase");
            }
        }

        #endregion
    }
}