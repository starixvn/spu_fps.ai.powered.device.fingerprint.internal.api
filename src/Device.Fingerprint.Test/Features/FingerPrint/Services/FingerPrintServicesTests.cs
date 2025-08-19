using Device.Fingerprint.Features.FingerPrint.Entities;
using Device.Fingerprint.Features.FingerPrint.Services;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.FingerPrint.Services
{
    public class FingerPrintServicesTests
    {
        private readonly Mock<IFingerPrintDataAccess> _mockDataAccess;
        private readonly FingerPrintServices _service;

        public FingerPrintServicesTests()
        {
            _mockDataAccess = new Mock<IFingerPrintDataAccess>();
            _service = new FingerPrintServices(_mockDataAccess.Object);
        }

        [Fact]
        public void GetFPFingerPrint_WithValidParameters_ShouldReturnFingerPrintData()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var batchSize = 100;
            ulong lastID = 1000;

            var expectedFingerPrints = new List<FPFingerPrintEntity>
            {
                new FPFingerPrintEntity 
                { 
                    ID = 1001, 
                    Code = "FP001", 
                    Attribute = "test-attribute-1", 
                    InsertedTime = DateTime.Now 
                },
                new FPFingerPrintEntity 
                { 
                    ID = 1002, 
                    Code = "FP002", 
                    Attribute = "test-attribute-2", 
                    InsertedTime = DateTime.Now 
                }
            };

            _mockDataAccess.Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID))
                          .Returns(expectedFingerPrints);

            // Act
            var result = _service.GetFPFingerPrint(fromDate, toDate, batchSize, lastID);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedFingerPrints);

            _mockDataAccess.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID), Times.Once);
        }

        [Fact]
        public void GetFPFingerPrint_WithEmptyResult_ShouldReturnEmptyCollection()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var batchSize = 100;
            ulong lastID = 1000;

            var emptyResult = new List<FPFingerPrintEntity>();

            _mockDataAccess.Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID))
                          .Returns(emptyResult);

            // Act
            var result = _service.GetFPFingerPrint(fromDate, toDate, batchSize, lastID);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockDataAccess.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID), Times.Once);
        }

        [Fact]
        public void GetFPFingerPrint_WithZeroBatchSize_ShouldStillCallDataAccess()
        {
            // Arrange
            var fromDate = new DateTime(2024, 1, 1);
            var toDate = new DateTime(2024, 1, 31);
            var batchSize = 0;
            ulong lastID = 0;

            var expectedResult = new List<FPFingerPrintEntity>();

            _mockDataAccess.Setup(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetFPFingerPrint(fromDate, toDate, batchSize, lastID);

            // Assert
            result.Should().NotBeNull();
            _mockDataAccess.Verify(x => x.GetFPFingerPrint(fromDate, toDate, batchSize, lastID), Times.Once);
        }

        [Fact]
        public void GetFPFingerPrint_WithSameDateRange_ShouldCallDataAccess()
        {
            // Arrange
            var sameDate = new DateTime(2024, 1, 15);
            var batchSize = 50;
            ulong lastID = 500;

            var expectedResult = new List<FPFingerPrintEntity>();

            _mockDataAccess.Setup(x => x.GetFPFingerPrint(sameDate, sameDate, batchSize, lastID))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetFPFingerPrint(sameDate, sameDate, batchSize, lastID);

            // Assert
            result.Should().NotBeNull();
            _mockDataAccess.Verify(x => x.GetFPFingerPrint(sameDate, sameDate, batchSize, lastID), Times.Once);
        }

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new FingerPrintServices(null));
        }
    }
}