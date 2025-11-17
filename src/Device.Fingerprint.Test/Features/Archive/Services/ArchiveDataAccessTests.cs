using Device.Fingerprint.Features.Archive.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Archive.Services
{
    /// <summary>
    /// Unit tests for ArchiveDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class ArchiveDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new ArchiveDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IArchiveDataAccess>();
        }

        [Fact]
        public void ArchiveDataAccess_ShouldImplementIArchiveDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new ArchiveDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IArchiveDataAccess>();
        }

    }
}
