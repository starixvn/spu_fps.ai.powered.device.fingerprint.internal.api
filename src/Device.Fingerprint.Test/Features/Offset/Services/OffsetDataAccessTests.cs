using Device.Fingerprint.Features.Offset.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Offset.Services
{
    /// <summary>
    /// Unit tests for OffsetDataAccess class.
    /// Note: OffsetDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class OffsetDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new OffsetDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IOffsetDataAccess>();
        }

        [Fact]
        public void OffsetDataAccess_ShouldImplementIOffsetDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new OffsetDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IOffsetDataAccess>();
        }

        [Fact]
        public void UpdateOffset_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(OffsetDataAccess).GetMethod("UpdateOffset");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }
    }
}
