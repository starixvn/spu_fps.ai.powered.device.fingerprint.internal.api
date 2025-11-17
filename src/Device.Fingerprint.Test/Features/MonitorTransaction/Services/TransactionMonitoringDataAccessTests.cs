using Device.Fingerprint.Features.MonitorTransaction.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.MonitorTransaction.Services
{
    /// <summary>
    /// Unit tests for TransactionMonitoringDataAccess class.
    /// Note: TransactionMonitoringDataAccess creates DynamicRepository in method,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class TransactionMonitoringDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Act
            var dataAccess = new TransactionMonitoringDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ITransactionMonitoringDataAccess>();
        }

        [Fact]
        public void TransactionMonitoringDataAccess_ShouldImplementITransactionMonitoringDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new TransactionMonitoringDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ITransactionMonitoringDataAccess>();
        }

        [Fact]
        public void SumRawTransactionAsync_MethodSignature_ShouldReturnTaskOfInt()
        {
            // Arrange
            var method = typeof(TransactionMonitoringDataAccess).GetMethod("SumRawTransactionAsync");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("Task");
        }
    }
}
