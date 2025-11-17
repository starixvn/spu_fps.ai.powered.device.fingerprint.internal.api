using Device.Fingerprint.Features.LogInfo.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.LogInfo.Services
{
    /// <summary>
    /// Unit tests for LogInfoDataAccess class.
    /// Note: LogInfoDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class LogInfoDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new LogInfoDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ILogInfoDataAccess>();
        }

        [Fact]
        public void LogInfoDataAccess_ShouldImplementILogInfoDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new LogInfoDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ILogInfoDataAccess>();
        }

        [Fact]
        public void InsertDCSConsumerLogAsync_MethodSignature_ShouldReturnTask()
        {
            // Arrange
            var method = typeof(LogInfoDataAccess).GetMethod("InsertDCSConsumerLogAsync");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Task");
        }
    }
}
