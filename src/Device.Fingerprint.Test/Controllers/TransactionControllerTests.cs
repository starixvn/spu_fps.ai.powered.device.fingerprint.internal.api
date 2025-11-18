using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.InternalAPI.Model;
using Device.Fingerprint.Features.Transaction.Model;
using Device.Fingerprint.Features.Transaction.Services;
using Fanex.Logging;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Device.Fingerprint.Test.Controllers
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<ILogger> _mockLogger;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _mockTransactionService = new Mock<ITransactionService>();
            _mockLogger = new Mock<ILogger>();
            _controller = new TransactionController(_mockTransactionService.Object, _mockLogger.Object);
        }

        #region GetTransactionInfo Tests

        [Fact]
        public void GetTransactionInfo_WithValidParameters_ShouldReturnTransactionInfoResponse()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 100;

            var transactionInfos = new List<TransactionInfoEntity>
            {
                new TransactionInfoEntity
                {
                    TransID = 1001,
                    CustID = 123,
                    SubscriberID = 1,
                    SubscriberName = "TestSubscriber",
                    loginName = "testuser",
                    Action = "LOGIN",
                    ActionResult = "SUCCESS",
                    IP = "192.168.1.1",
                    DeviceCode = "DEVICE001",
                    BrowserName = "Chrome",
                    BrowserVersion = "120.0",
                    OSName = "Windows",
                    OSVersion = "10",
                    TransTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    InsertTime = DateTime.Now
                }
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(transactionInfos);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            response.TransactionInfo.Should().NotBeNull();
            response.TransactionInfo.Should().HaveCount(1);
            
            var transactionInfo = response.TransactionInfo.First();
            transactionInfo.TransID.Should().Be(1001);
            transactionInfo.CustID.Should().Be(123);
            transactionInfo.SubscriberID.Should().Be(1);
            transactionInfo.LoginName.Should().Be("testuser");
            transactionInfo.Action.Should().Be("LOGIN");
            transactionInfo.ActionResult.Should().Be("SUCCESS");
        }

        [Fact]
        public void GetTransactionInfo_WithZeroLastTransID_ShouldCallService()
        {
            // Arrange
            ulong lastTransID = 0; // Note: ulong can't be negative, so the < 0 check in controller is unreachable
            int batchSize = 100;

            var transactionInfos = new List<TransactionInfoEntity>();

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(transactionInfos);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            // Since ulong can't be negative, the service will be called
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _mockTransactionService.Verify(x => x.GetTransactionInfo(lastTransID, batchSize), Times.Once);
        }

        [Fact]
        public void GetTransactionInfo_WithBatchSizeGreaterThan10000_ShouldReturnBadRequest()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 10001;

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be("batchSize cannot be greater than 10000!");
            response.TransactionInfo.Should().BeNull();
            
            _mockTransactionService.Verify(x => x.GetTransactionInfo(It.IsAny<ulong>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void GetTransactionInfo_WithBatchSizeEqualTo10000_ShouldReturnSuccess()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 10000;

            var transactionInfos = new List<TransactionInfoEntity>();

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(transactionInfos);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            _mockTransactionService.Verify(x => x.GetTransactionInfo(lastTransID, batchSize), Times.Once);
        }

        [Fact]
        public void GetTransactionInfo_WithEmptyResults_ShouldReturnEmptyResponse()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 100;

            var emptyResults = new List<TransactionInfoEntity>();

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(emptyResults);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            response.TransactionInfo.Should().BeNull();
        }

        [Fact]
        public void GetTransactionInfo_WithNullResults_ShouldReturnEmptyResponse()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 100;

            IEnumerable<TransactionInfoEntity> nullResults = null!;

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(nullResults);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public void GetTransactionInfo_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 100;
            string errorMessage = "Database connection failed";

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Throws(new Exception(errorMessage));

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void GetTransactionInfo_ShouldTransformEntityToModel()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 100;

            var transactionInfos = new List<TransactionInfoEntity>
            {
                new TransactionInfoEntity
                {
                    TransID = 1001,
                    CustID = 123,
                    SubscriberID = 1,
                    SubscriberName = "TestSubscriber",
                    loginName = "testuser",
                    Action = "LOGIN",
                    ActionResult = "SUCCESS",
                    ActionResultID = 1,
                    BotComponentID = 2,
                    BotDetectionValue = 100,
                    BrowserName = "Chrome",
                    BrowserVersion = "120.0",
                    OSName = "Windows",
                    OSVersion = "10",
                    DeviceType = "Desktop",
                    IP = "192.168.1.1",
                    IPID = "IP001",
                    URLID = 500,
                    URLDetails = "https://example.com",
                    UserAgent = "Mozilla/5.0",
                    UserAgentKey = "UA001",
                    RobotTracking = "No",
                    TransStatus = "Active",
                    TransTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    InsertTime = DateTime.Now,
                    Flagged = 0,
                    FakeIP = "No",
                    DeviceID = 100,
                    DeviceCode = "DEVICE001",
                    FirstDeviceCode = "FIRST001",
                    DeviceStatus = 1,
                    DeviceFingerprintID = 200,
                    FingerprintCode = "FP001"
                }
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(transactionInfos);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<TransactionInfoResponse>().Subject;
            
            var model = response.TransactionInfo.First();
            model.TransID.Should().Be(transactionInfos[0].TransID);
            model.CustID.Should().Be(transactionInfos[0].CustID);
            model.SubscriberID.Should().Be(transactionInfos[0].SubscriberID);
            model.SubscriberName.Should().Be(transactionInfos[0].SubscriberName);
            model.LoginName.Should().Be(transactionInfos[0].loginName);
            model.Action.Should().Be(transactionInfos[0].Action);
            model.ActionResult.Should().Be(transactionInfos[0].ActionResult);
            model.ActionResultID.Should().Be(transactionInfos[0].ActionResultID);
            model.BotComponentID.Should().Be(transactionInfos[0].BotComponentID);
            model.BotDetectionValue.Should().Be(transactionInfos[0].BotDetectionValue);
            model.BrowserName.Should().Be(transactionInfos[0].BrowserName);
            model.BrowserVersion.Should().Be(transactionInfos[0].BrowserVersion);
            model.OSName.Should().Be(transactionInfos[0].OSName);
            model.OSVersion.Should().Be(transactionInfos[0].OSVersion);
            model.DeviceType.Should().Be(transactionInfos[0].DeviceType);
            model.IP.Should().Be(transactionInfos[0].IP);
            model.IPID.Should().Be(transactionInfos[0].IPID);
            model.URLID.Should().Be(transactionInfos[0].URLID);
            model.URLDetails.Should().Be(transactionInfos[0].URLDetails);
            model.UserAgent.Should().Be(transactionInfos[0].UserAgent);
            model.UserAgentKey.Should().Be(transactionInfos[0].UserAgentKey);
            model.RobotTracking.Should().Be(transactionInfos[0].RobotTracking);
            model.TransStatus.Should().Be(transactionInfos[0].TransStatus);
            model.FakeIP.Should().Be(transactionInfos[0].FakeIP);
            model.DeviceID.Should().Be(transactionInfos[0].DeviceID);
            model.DeviceCode.Should().Be(transactionInfos[0].DeviceCode);
            model.FirstDeviceCode.Should().Be(transactionInfos[0].FirstDeviceCode);
            model.DeviceStatus.Should().Be(transactionInfos[0].DeviceStatus);
            model.DeviceFingerprintID.Should().Be(transactionInfos[0].DeviceFingerprintID);
            model.FingerprintCode.Should().Be(transactionInfos[0].FingerprintCode);
        }

        #endregion

        #region GetTransactionHistory Tests

        [Fact]
        public void GetTransactionHistory_WithValidParam_ShouldReturnTransactionResult()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                SubscriberID = 1,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                UserName = "testuser",
                IsPartial = false,
                Skip = 0,
                Take = 10
            };

            var expectedResult = new TransactionResultModel
            {
                TotalItem = 1,
                Items = new List<TransactionItemModel>
                {
                    new TransactionItemModel
                    {
                        SubscriberID = 1,
                        UserName = "testuser",
                        Action = "LOGIN",
                        ActionResult = "SUCCESS"
                    }
                }
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistory(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.GetTransactionHistory(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var transactionResult = okResult.Value.Should().BeOfType<TransactionResultModel>().Subject;
            
            transactionResult.TotalItem.Should().Be(expectedResult.TotalItem);
            transactionResult.Items.Should().HaveCount(1);
            transactionResult.Items.First().UserName.Should().Be("testuser");
            
            _mockTransactionService.Verify(x => x.GetTransactionHistory(param), Times.Once);
        }

        [Fact]
        public void GetTransactionHistory_WhenServiceThrowsException_ShouldReturnEmptyResult()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                SubscriberID = 1,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistory(param))
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetTransactionHistory(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var transactionResult = okResult.Value.Should().BeOfType<TransactionResultModel>().Subject;
            
            transactionResult.Should().NotBeNull();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Transaction History exception"))), Times.Once);
        }

        [Fact]
        public void GetTransactionHistory_WhenServiceThrowsTimeoutException_ShouldReturnTimeoutResult()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                SubscriberID = 1,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistory(param))
                .Throws(new Exception("timeout expired"));

            // Act
            var result = _controller.GetTransactionHistory(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var transactionResult = okResult.Value.Should().BeOfType<TransactionResultModel>().Subject;
            
            transactionResult.IsTimeout.Should().BeTrue();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Transaction History exception"))), Times.Once);
        }

        #endregion

        #region GetLastestTransactionBySubscriber Tests

        [Fact]
        public void GetLastestTransactionBySubscriber_WithValidParam_ShouldReturnLatestTransactionResult()
        {
            // Arrange
            var param = new LatestTransactionParamModel
            {
                SubscriberID = 1,
                Length = 10,
                Skip = 0,
                Take = 10
            };

            var expectedResult = new LatestTransactionResultModel
            {
                TotalItem = 1,
                Items = new List<LatestTransactionItemModel>
                {
                    new LatestTransactionItemModel
                    {
                        SubscriberID = 1,
                        UserName = "testuser",
                        Action = "LOGIN",
                        ActionResult = "SUCCESS",
                        TransID = 1001
                    }
                }
            };

            _mockTransactionService
                .Setup(x => x.GetLastestTransactionBySubscriber(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.GetLastestTransactionBySubscriber(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var latestResult = okResult.Value.Should().BeOfType<LatestTransactionResultModel>().Subject;
            
            latestResult.TotalItem.Should().Be(expectedResult.TotalItem);
            latestResult.Items.Should().HaveCount(1);
            latestResult.Items.First().UserName.Should().Be("testuser");
            
            _mockTransactionService.Verify(x => x.GetLastestTransactionBySubscriber(param), Times.Once);
        }

        [Fact]
        public void GetLastestTransactionBySubscriber_WhenServiceThrowsException_ShouldReturnEmptyResult()
        {
            // Arrange
            var param = new LatestTransactionParamModel
            {
                SubscriberID = 1,
                Length = 10,
                Skip = 0,
                Take = 10
            };

            _mockTransactionService
                .Setup(x => x.GetLastestTransactionBySubscriber(param))
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetLastestTransactionBySubscriber(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var latestResult = okResult.Value.Should().BeOfType<LatestTransactionResultModel>().Subject;
            
            latestResult.Should().NotBeNull();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Lastest Transaction By Subscriber exception"))), Times.Once);
        }

        [Fact]
        public void GetLastestTransactionBySubscriber_WithEmptyResults_ShouldReturnEmptyResult()
        {
            // Arrange
            var param = new LatestTransactionParamModel
            {
                SubscriberID = 1,
                Length = 10,
                Skip = 0,
                Take = 10
            };

            var emptyResult = new LatestTransactionResultModel
            {
                TotalItem = 0,
                Items = new List<LatestTransactionItemModel>()
            };

            _mockTransactionService
                .Setup(x => x.GetLastestTransactionBySubscriber(param))
                .Returns(emptyResult);

            // Act
            var result = _controller.GetLastestTransactionBySubscriber(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var latestResult = okResult.Value.Should().BeOfType<LatestTransactionResultModel>().Subject;
            
            latestResult.TotalItem.Should().Be(0);
            latestResult.Items.Should().BeEmpty();
        }

        #endregion

        #region GetTransactionHistoryByDevice Tests

        [Fact]
        public void GetTransactionHistoryByDevice_WithValidParam_ShouldReturnTransactionHistoryByDeviceResult()
        {
            // Arrange
            var param = new TransactionHistoryByDeviceParamModel
            {
                SubscriberID = 1,
                DeviceID = 12345,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                Skip = 0,
                Take = 10
            };

            var expectedResult = new TransactionHistoryByDeviceResultModel
            {
                TotalItem = 1,
                Items = new List<TransactionHistoryByDeviceItemModel>
                {
                    new TransactionHistoryByDeviceItemModel
                    {
                        TransID = 1001,
                        UserName = "testuser",
                        Action = "LOGIN",
                        ActionResult = "SUCCESS",
                        RegisterName = "TestRegister",
                        OS = "Windows",
                        Browser = "Chrome",
                        IP = "192.168.1.1"
                    }
                }
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistoryByDevice(param))
                .Returns(expectedResult);

            // Act
            var result = _controller.GetTransactionHistoryByDevice(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var deviceResult = okResult.Value.Should().BeOfType<TransactionHistoryByDeviceResultModel>().Subject;
            
            deviceResult.TotalItem.Should().Be(expectedResult.TotalItem);
            deviceResult.Items.Should().HaveCount(1);
            deviceResult.Items.First().UserName.Should().Be("testuser");
            deviceResult.Items.First().Action.Should().Be("LOGIN");
            
            _mockTransactionService.Verify(x => x.GetTransactionHistoryByDevice(param), Times.Once);
        }

        [Fact]
        public void GetTransactionHistoryByDevice_WhenServiceThrowsException_ShouldReturnEmptyResult()
        {
            // Arrange
            var param = new TransactionHistoryByDeviceParamModel
            {
                SubscriberID = 1,
                DeviceID = 12345,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistoryByDevice(param))
                .Throws(new Exception("Database error"));

            // Act
            var result = _controller.GetTransactionHistoryByDevice(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var deviceResult = okResult.Value.Should().BeOfType<TransactionHistoryByDeviceResultModel>().Subject;
            
            deviceResult.Should().NotBeNull();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Transaction History By Device exception"))), Times.Once);
        }

        [Fact]
        public void GetTransactionHistoryByDevice_WhenServiceThrowsTimeoutException_ShouldReturnTimeoutResult()
        {
            // Arrange
            var param = new TransactionHistoryByDeviceParamModel
            {
                SubscriberID = 1,
                DeviceID = 12345,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistoryByDevice(param))
                .Throws(new Exception("timeout expired"));

            // Act
            var result = _controller.GetTransactionHistoryByDevice(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var deviceResult = okResult.Value.Should().BeOfType<TransactionHistoryByDeviceResultModel>().Subject;
            
            deviceResult.IsTimeout.Should().BeTrue();
            _mockLogger.Verify(x => x.Error(It.Is<string>(s => s.Contains("FPS Get Transaction History By Device exception"))), Times.Once);
        }

        [Fact]
        public void GetTransactionHistoryByDevice_WithEmptyResults_ShouldReturnEmptyResult()
        {
            // Arrange
            var param = new TransactionHistoryByDeviceParamModel
            {
                SubscriberID = 1,
                DeviceID = 12345,
                FromDate = DateTime.Now.AddDays(-7),
                ToDate = DateTime.Now,
                Skip = 0,
                Take = 10
            };

            var emptyResult = new TransactionHistoryByDeviceResultModel
            {
                TotalItem = 0,
                Items = new List<TransactionHistoryByDeviceItemModel>()
            };

            _mockTransactionService
                .Setup(x => x.GetTransactionHistoryByDevice(param))
                .Returns(emptyResult);

            // Act
            var result = _controller.GetTransactionHistoryByDevice(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var deviceResult = okResult.Value.Should().BeOfType<TransactionHistoryByDeviceResultModel>().Subject;
            
            deviceResult.TotalItem.Should().Be(0);
            deviceResult.Items.Should().BeEmpty();
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void GetTransactionInfo_WithZeroBatchSize_ShouldCallService()
        {
            // Arrange
            ulong lastTransID = 1000;
            int batchSize = 0;

            var transactionInfos = new List<TransactionInfoEntity>();

            _mockTransactionService
                .Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(transactionInfos);

            // Act
            var result = _controller.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().NotBeNull();
            _mockTransactionService.Verify(x => x.GetTransactionInfo(lastTransID, batchSize), Times.Once);
        }

        [Fact]
        public void GetTransactionHistory_WithNullParam_ShouldHandleGracefully()
        {
            // Arrange
            TransactionParamModel param = null!;

            _mockTransactionService
                .Setup(x => x.GetTransactionHistory(param))
                .Throws(new ArgumentNullException());

            // Act
            var result = _controller.GetTransactionHistory(param);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeOfType<TransactionResultModel>();
        }

        #endregion
    }
}

