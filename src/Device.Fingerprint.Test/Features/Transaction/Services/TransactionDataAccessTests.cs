using Device.Fingerprint.Features.Transaction.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Transaction.Services
{
    /// <summary>
    /// Unit tests for TransactionDataAccess class.
    /// Note: TransactionDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests only.
    /// </summary>
    public class TransactionDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new TransactionDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ITransactionDataAccess>();
        }

        [Fact]
        public void TransactionDataAccess_ShouldImplementITransactionDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new TransactionDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ITransactionDataAccess>();
        }

        [Fact]
        public void TransactionDataAccess_ShouldHaveGetTransactionByCustSubscriberMethod()
        {
            // Arrange
            var method = typeof(TransactionDataAccess).GetMethod("GetTransactionByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void TransactionDataAccess_ShouldHaveGetTransactionBySubscriberMethod()
        {
            // Arrange
            var method = typeof(TransactionDataAccess).GetMethod("GetTransactionBySubscriber");

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void TransactionDataAccess_ShouldHaveGetLastestTransactionBySubscriberMethod()
        {
            // Arrange
            var method = typeof(TransactionDataAccess).GetMethod("GetLastestTransactionBySubscriber");

            // Assert
            method.Should().NotBeNull();
        }

        [Fact]
        public void TransactionDataAccess_ShouldHaveGetTransactionHistoryByDeviceMethod()
        {
            // Arrange
            var method = typeof(TransactionDataAccess).GetMethod("GetTransactionHistoryByDevice");

            // Assert
            method.Should().NotBeNull();
        }
    }
}
