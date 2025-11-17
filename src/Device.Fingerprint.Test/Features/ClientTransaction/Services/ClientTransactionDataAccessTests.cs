using Device.Fingerprint.Features.ClientTransaction.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.ClientTransaction.Services
{
    /// <summary>
    /// Unit tests for ClientTransactionDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class ClientTransactionDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new ClientTransactionDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IClientTransactionDataAccess>();
        }

        [Fact]
        public void ClientTransactionDataAccess_ShouldImplementIClientTransactionDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new ClientTransactionDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IClientTransactionDataAccess>();
        }

    }
}
