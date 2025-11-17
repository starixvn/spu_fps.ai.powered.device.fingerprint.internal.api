using Device.Fingerprint.Features.LogTrail.Services;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.LogTrail.Services
{
    /// <summary>
    /// Unit tests for LogTrailServices class.
    /// </summary>
    public class LogTrailServicesTests
    {
        private readonly Mock<ILogTrailDataAccess> _mockDataAccess;
        private readonly LogTrailServices _service;

        public LogTrailServicesTests()
        {
            _mockDataAccess = new Mock<ILogTrailDataAccess>();
            _service = new LogTrailServices(_mockDataAccess.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeDataAccess()
        {
            // Act
            var service = new LogTrailServices(_mockDataAccess.Object);

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public async Task InsertSummaryLogTrailAsync_ShouldCallDataAccessAndReturnResult()
        {
            // Arrange
            var rawLogEvent = "{\"eventType\":\"LOGIN\",\"timestamp\":\"2025-11-17\"}";
            var expectedResult = 1;
            _mockDataAccess.Setup(x => x.InsertSummaryLogTrailAsync(It.IsAny<string>()))
                          .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.InsertSummaryLogTrailAsync(rawLogEvent);

            // Assert
            result.Should().Be(expectedResult);
            _mockDataAccess.Verify(x => x.InsertSummaryLogTrailAsync(rawLogEvent), Times.Once);
        }

        [Fact]
        public async Task InsertSummaryLogTrailAsync_WithEmptyJson_ShouldStillCallDataAccess()
        {
            // Arrange
            var rawLogEvent = "{}";
            _mockDataAccess.Setup(x => x.InsertSummaryLogTrailAsync(It.IsAny<string>()))
                          .ReturnsAsync(0);

            // Act
            var result = await _service.InsertSummaryLogTrailAsync(rawLogEvent);

            // Assert
            _mockDataAccess.Verify(x => x.InsertSummaryLogTrailAsync(rawLogEvent), Times.Once);
        }
    }
}
