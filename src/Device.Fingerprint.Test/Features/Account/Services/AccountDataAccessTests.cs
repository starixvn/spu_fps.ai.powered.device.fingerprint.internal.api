using Device.Fingerprint.Features.Account.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Account.Services
{
    /// <summary>
    /// Unit tests for AccountDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class AccountDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new AccountDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IAccountDataAccess>();
        }

        [Fact]
        public void AccountDataAccess_ShouldImplementIAccountDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new AccountDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IAccountDataAccess>();
        }
    }
}
