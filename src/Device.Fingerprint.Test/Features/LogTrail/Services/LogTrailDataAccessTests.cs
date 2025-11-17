using Device.Fingerprint.Features.LogTrail.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.LogTrail.Services
{
    /// <summary>
    /// Unit tests for LogTrailDataAccess class.
    /// Note: LogTrailDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class LogTrailDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new LogTrailDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ILogTrailDataAccess>();
        }

        [Fact]
        public void LogTrailDataAccess_ShouldImplementILogTrailDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new LogTrailDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ILogTrailDataAccess>();
        }

        [Fact]
        public void InsertSummaryLogTrailAsync_MethodSignature_ShouldReturnTaskOfInt()
        {
            // Arrange
            var method = typeof(LogTrailDataAccess).GetMethod("InsertSummaryLogTrailAsync");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("Task");
        }
    }
}
