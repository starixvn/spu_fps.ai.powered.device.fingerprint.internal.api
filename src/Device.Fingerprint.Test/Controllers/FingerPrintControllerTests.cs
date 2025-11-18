using Device.Fingerprint.InternalAPI.Controllers;
using Device.Fingerprint.InternalAPI.Model;
using Device.Fingerprint.InternalAPI.Services;
using Device.Fingerprint.Features.FingerPrint.Entities;
using Device.Fingerprint.Features.FingerPrint.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Device.Fingerprint.Test.Controllers
{
    public class FingerPrintControllerTests
    {
        private readonly Mock<IFingerPrintServices> _mockFingerPrintServices;
        private readonly Mock<ILogstashFPFingerprintService> _mockLogstashService;
        private readonly FingerPrintController _controller;

        public FingerPrintControllerTests()
        {
            _mockFingerPrintServices = new Mock<IFingerPrintServices>();
            _mockLogstashService = new Mock<ILogstashFPFingerprintService>();
            _controller = new FingerPrintController(_mockFingerPrintServices.Object, _mockLogstashService.Object);
        }

        #region GetFingerPrint Tests

        [Fact]
        public void GetFingerPrint_WithValidParameters_ShouldReturnFingerPrintResponse()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "{\"Platform\":\"Windows\",\"BrowserName\":\"Chrome\"}",
                    InsertedTime = DateTime.Now
                },
                new FPFingerPrintEntity
                {
                    ID = 1002,
                    Code = "FP002",
                    Attribute = "{\"Platform\":\"MacOS\",\"BrowserName\":\"Safari\"}",
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            response.FPFingerPrint.Should().NotBeNull();
            response.FPFingerPrint.Should().HaveCount(2);
            
            var fingerPrintList = response.FPFingerPrint!.ToList();
            fingerPrintList.Should().Contain(f => f.ID == 1001 && f.Code == "FP001");
            fingerPrintList.Should().Contain(f => f.ID == 1002 && f.Code == "FP002");
            fingerPrintList.All(f => f.Attribute != null).Should().BeTrue();
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
        }

        [Fact]
        public void GetFingerPrint_WithFromDateGreaterThanToDate_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now.AddDays(-7); // Invalid: fromDate > toDate
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be("FromDate cannot be greater than ToDate!");
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ulong>()), Times.Never);
        }

        [Fact]
        public void GetFingerPrint_WithBatchSizeGreaterThan10000_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 10001;
            ulong lastFingerPrintID = 1000;

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be("BatchSize cannot be greater than 10000!");
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ulong>()), Times.Never);
        }

        [Fact]
        public void GetFingerPrint_WithBatchSizeEqualTo10000_ShouldReturnSuccess()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 10000;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ErrorMessage.Should().Be("No Fingerprint Found!");
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
        }

        [Fact]
        public void GetFingerPrint_WithEmptyResults_ShouldReturnNotFound()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var emptyResults = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(emptyResults);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ErrorMessage.Should().Be("No Fingerprint Found!");
            response.FPFingerPrint.Should().NotBeNull();
            response.FPFingerPrint.Should().BeEmpty();
        }

        [Fact]
        public void GetFingerPrint_WithNullResults_ShouldReturnNotFound()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            IEnumerable<FPFingerPrintEntity> nullResults = null!;

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(nullResults);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.ErrorMessage.Should().Be("No Fingerprint Found!");
        }

        [Fact]
        public void GetFingerPrint_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            string errorMessage = "Database connection failed";

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Throws(new Exception(errorMessage));

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void GetFingerPrint_ShouldTransformFingerPrintEntities()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            var insertedTime = DateTime.Now;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "{\"Platform\":\"Windows\",\"BrowserName\":\"Chrome\",\"Fonts\":\"Arial\"}",
                    InsertedTime = insertedTime
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.FPFingerPrint.Should().NotBeNull();
            var transformed = response.FPFingerPrint!.First();
            transformed.ID.Should().Be(fingerPrints[0].ID);
            transformed.Code.Should().Be(fingerPrints[0].Code);
            transformed.InsertedTime.Should().Be(fingerPrints[0].InsertedTime);
            transformed.Attribute.Should().NotBeNull();
        }

        [Fact]
        public void GetFingerPrint_WithEmptyAttribute_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = string.Empty,
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.FPFingerPrint.Should().NotBeNull();
            var transformed = response.FPFingerPrint!.First();
            transformed.Attribute.Should().NotBeNull();
        }

        [Fact]
        public void GetFingerPrint_WithNullAttribute_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = null!,
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.FPFingerPrint.Should().NotBeNull();
            var transformed = response.FPFingerPrint!.First();
            transformed.Attribute.Should().NotBeNull();
        }

        [Fact]
        public void GetFingerPrint_WithMultipleFingerPrints_ShouldTransformAll()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity { ID = 1001, Code = "FP001", Attribute = "{}", InsertedTime = DateTime.Now },
                new FPFingerPrintEntity { ID = 1002, Code = "FP002", Attribute = "{}", InsertedTime = DateTime.Now },
                new FPFingerPrintEntity { ID = 1003, Code = "FP003", Attribute = "{}", InsertedTime = DateTime.Now }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.FPFingerPrint.Should().NotBeNull();
            response.FPFingerPrint.Should().HaveCount(3);
            response.FPFingerPrint!.Select(f => f.ID).Should().Contain(new[] { 1001UL, 1002UL, 1003UL });
        }

        #endregion

        #region PushFingerprintToLogstash Tests

        [Fact]
        public void PushFingerprintToLogstash_WithValidParameters_ShouldPushToLogstash()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            ulong maxFingerprintId = 2000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "{\"Platform\":\"Windows\"}",
                    InsertedTime = DateTime.Now
                },
                new FPFingerPrintEntity
                {
                    ID = 2000,
                    Code = "FP002",
                    Attribute = "{\"Platform\":\"MacOS\"}",
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            _mockLogstashService
                .Setup(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()))
                .Returns(maxFingerprintId);

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            response.MaxFingerprintID.Should().Be(maxFingerprintId);
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
            _mockLogstashService.Verify(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()), Times.Once);
        }

        [Fact]
        public void PushFingerprintToLogstash_WithFromDateGreaterThanToDate_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now;
            var toDate = DateTime.Now.AddDays(-7);
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be("FromDate cannot be greater than ToDate!");
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ulong>()), Times.Never);
            _mockLogstashService.Verify(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()), Times.Never);
        }

        [Fact]
        public void PushFingerprintToLogstash_WithBatchSizeGreaterThan10000_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 10001;
            ulong lastFingerPrintID = 1000;

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be("BatchSize cannot be greater than 10000!");
            
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<ulong>()), Times.Never);
            _mockLogstashService.Verify(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()), Times.Never);
        }

        [Fact]
        public void PushFingerprintToLogstash_WithEmptyResults_ShouldReturnSuccessWithoutPushing()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var emptyResults = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(emptyResults);

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            response.MaxFingerprintID.Should().Be(0);
            
            _mockLogstashService.Verify(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()), Times.Never);
        }

        [Fact]
        public void PushFingerprintToLogstash_WithNullResults_ShouldReturnSuccessWithoutPushing()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            IEnumerable<FPFingerPrintEntity> nullResults = null!;

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(nullResults);

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.ErrorMessage.Should().BeEmpty();
            
            _mockLogstashService.Verify(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()), Times.Never);
        }

        [Fact]
        public void PushFingerprintToLogstash_WhenServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            string errorMessage = "Database connection failed";

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Throws(new Exception(errorMessage));

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void PushFingerprintToLogstash_WhenLogstashServiceThrowsException_ShouldReturnBadRequest()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            string errorMessage = "Logstash connection failed";

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "{}",
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            _mockLogstashService
                .Setup(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()))
                .Throws(new Exception(errorMessage));

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            response.ErrorMessage.Should().Be(errorMessage);
        }

        [Fact]
        public void PushFingerprintToLogstash_ShouldReturnMaxFingerprintID()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;
            ulong expectedMaxId = 5000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity { ID = 1001, Code = "FP001", Attribute = "{}", InsertedTime = DateTime.Now },
                new FPFingerPrintEntity { ID = 5000, Code = "FP002", Attribute = "{}", InsertedTime = DateTime.Now },
                new FPFingerPrintEntity { ID = 2000, Code = "FP003", Attribute = "{}", InsertedTime = DateTime.Now }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            _mockLogstashService
                .Setup(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()))
                .Returns(expectedMaxId);

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.MaxFingerprintID.Should().Be(expectedMaxId);
            _mockLogstashService.Verify(x => x.PushLogAsync(It.Is<IEnumerable<FPFingerPrintTransform>>(list => list.Count() == 3)), Times.Once);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void GetFingerPrint_WithSameFromDateAndToDate_ShouldReturnSuccess()
        {
            // Arrange
            var fromDate = DateTime.Now.Date;
            var toDate = DateTime.Now.Date;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
        }

        [Fact]
        public void GetFingerPrint_WithZeroBatchSize_ShouldCallService()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 0;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
        }

        [Fact]
        public void GetFingerPrint_WithZeroLastFingerPrintID_ShouldCallService()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 0;

            var fingerPrints = new List<FPFingerPrintEntity>();

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            _mockFingerPrintServices.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID), Times.Once);
        }

        [Fact]
        public void GetFingerPrint_WithInvalidJsonAttribute_ShouldHandleGracefully()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "invalid json {",
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            // Act
            var result = _controller.GetFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            // Should handle gracefully - either return BadRequest or transform with empty/null attribute
            response.Should().NotBeNull();
        }

        [Fact]
        public void PushFingerprintToLogstash_WithZeroMaxFingerprintID_ShouldReturnZero()
        {
            // Arrange
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            int batchSize = 100;
            ulong lastFingerPrintID = 1000;

            var fingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity
                {
                    ID = 1001,
                    Code = "FP001",
                    Attribute = "{}",
                    InsertedTime = DateTime.Now
                }
            };

            _mockFingerPrintServices
                .Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastFingerPrintID))
                .Returns(fingerPrints);

            _mockLogstashService
                .Setup(x => x.PushLogAsync(It.IsAny<IEnumerable<FPFingerPrintTransform>>()))
                .Returns(0UL);

            // Act
            var result = _controller.PushFingerprintToLogstash(fromDate, toDate, batchSize, lastFingerPrintID);

            // Assert
            result.Should().NotBeNull();
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<FPFingerPrintResponse>().Subject;
            
            response.MaxFingerprintID.Should().Be(0);
        }

        #endregion
    }
}

