using Device.Fingerprint.Features.LogInfo.Model;
using Device.Fingerprint.Features.LogInfo.Services;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.LogInfo.Services
{
    public class LogInfoServicesTests
    {
        private readonly Mock<ILogInfoDataAccess> _mockDataAccess;
        private readonly LogInfoServices _service;

        public LogInfoServicesTests()
        {
            _mockDataAccess = new Mock<ILogInfoDataAccess>();
            _service = new LogInfoServices(_mockDataAccess.Object);
        }

        #region InsertDCSConsumerLogAsync Tests

        [Fact]
        public async Task InsertDCSConsumerLogAsync_WithValidLogInfo_ShouldCallDataAccess()
        {
            // Arrange
            var logInfo = new LogInfoModel
            {
                LogType = 1,
                Environment = "Test",
                ScriptVersion = "1.0.0",
                SessionKey = "session123",
                Tagging = "tag1",
                LoginName = "testuser",
                SubscriberName = "testsubscriber",
                TransTime = DateTime.Now,
                UserAgent = "Mozilla/5.0",
                URL = "https://example.com"
            };

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Returns(Task.CompletedTask)
                          .Verifiable();

            // Act
            await _service.InsertDCSConsumerLogAsync(logInfo);

            // Assert
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InsertDCSConsumerLogAsync_WithNullLogInfo_ShouldCallDataAccess()
        {
            // Arrange
            LogInfoModel logInfo = null;

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Returns(Task.CompletedTask)
                          .Verifiable();

            // Act
            await _service.InsertDCSConsumerLogAsync(logInfo);

            // Assert
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InsertDCSConsumerLogAsync_WithMinimalLogInfo_ShouldCallDataAccess()
        {
            // Arrange
            var logInfo = new LogInfoModel
            {
                LogType = 0,
                TransTime = DateTime.MinValue
            };

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Returns(Task.CompletedTask)
                          .Verifiable();

            // Act
            await _service.InsertDCSConsumerLogAsync(logInfo);

            // Assert
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InsertDCSConsumerLogAsync_WhenDataAccessThrows_ShouldPropagateException()
        {
            // Arrange
            var logInfo = new LogInfoModel
            {
                LogType = 1,
                Environment = "Test"
            };
            var expectedException = new InvalidOperationException("Database error");

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .ThrowsAsync(expectedException);

            // Act & Assert
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.InsertDCSConsumerLogAsync(logInfo));

            actualException.Should().BeSameAs(expectedException);
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task InsertDCSConsumerLogAsync_WithComplexLogInfo_ShouldCallDataAccess()
        {
            // Arrange
            var logInfo = new LogInfoModel
            {
                LogType = 2,
                Environment = "Production",
                ScriptVersion = "2.1.5",
                SessionKey = "complex-session-key-12345",
                Tagging = "error,critical,authentication",
                LoginName = "complex.user@domain.com",
                SubscriberName = "Enterprise Subscriber",
                TransTime = new DateTime(2024, 6, 15, 14, 30, 0),
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",
                URL = "https://secure.example.com/api/v2/authenticate?redirect=dashboard"
            };

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Returns(Task.CompletedTask)
                          .Verifiable();

            // Act
            await _service.InsertDCSConsumerLogAsync(logInfo);

            // Assert
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new LogInfoServices(null));
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task InsertDCSConsumerLogAsync_MultipleCallsInParallel_ShouldCallDataAccessForEach()
        {
            // Arrange
            var logInfos = new[]
            {
                new LogInfoModel { LogType = 1, Environment = "Test1", TransTime = DateTime.Now },
                new LogInfoModel { LogType = 2, Environment = "Test2", TransTime = DateTime.Now },
                new LogInfoModel { LogType = 3, Environment = "Test3", TransTime = DateTime.Now }
            };

            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Returns(Task.CompletedTask);

            // Act
            var tasks = logInfos.Select(logInfo => _service.InsertDCSConsumerLogAsync(logInfo));
            await Task.WhenAll(tasks);

            // Assert
            _mockDataAccess.Verify(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()), Times.Exactly(3));
        }

        #endregion

        #region JSON Serialization Tests

        [Fact]
        public async Task InsertDCSConsumerLogAsync_ShouldSerializeLogInfoCorrectly()
        {
            // Arrange
            var logInfo = new LogInfoModel
            {
                LogType = 1,
                Environment = "Test",
                LoginName = "user@test.com",
                TransTime = new DateTime(2024, 1, 1, 12, 0, 0)
            };

            string capturedJson = null;
            _mockDataAccess.Setup(x => x.InsertDCSConsumerLogAsync(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json)
                          .Returns(Task.CompletedTask);

            // Act
            await _service.InsertDCSConsumerLogAsync(logInfo);

            // Assert
            capturedJson.Should().NotBeNull();
            capturedJson.Should().Contain("LogType");
            capturedJson.Should().Contain("Environment");
            capturedJson.Should().Contain("user@test.com");
            // Should be wrapped in array brackets
            capturedJson.Should().StartWith("[");
            capturedJson.Should().EndWith("]");
        }

        #endregion
    }
}