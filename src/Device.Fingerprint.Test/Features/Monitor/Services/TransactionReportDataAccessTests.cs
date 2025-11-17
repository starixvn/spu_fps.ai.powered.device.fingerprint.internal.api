using Device.Fingerprint.Features.Monitor.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Monitor.Services
{
    /// <summary>
    /// Unit tests for TransactionReportDataAccess class.
    /// Note: TransactionReportDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class TransactionReportDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new TransactionReportDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ITransactionReportDataAccess>();
        }

        [Fact]
        public void TransactionReportDataAccess_ShouldImplementITransactionReportDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new TransactionReportDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ITransactionReportDataAccess>();
        }

        [Fact]
        public void GetTransactionSumList_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(TransactionReportDataAccess).GetMethod("GetTransactionSumList");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }
    }
}
