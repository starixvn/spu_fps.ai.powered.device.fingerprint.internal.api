using Device.Fingerprint.Features.SummaryTransaction.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.SummaryTransaction.Services
{
    /// <summary>
    /// Unit tests for SummaryTransactionDataAccess class.
    /// Note: SummaryTransactionDataAccess initializes DynamicRepository in constructor,
    /// making it difficult to mock. These are basic instantiation tests.
    /// </summary>
    public class SummaryTransactionDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new SummaryTransactionDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<ISummaryTransactionDataAccess>();
        }

        [Fact]
        public void SummaryTransactionDataAccess_ShouldImplementISummaryTransactionDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new SummaryTransactionDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<ISummaryTransactionDataAccess>();
        }

        [Fact]
        public void SummaryTransactionDaily_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(SummaryTransactionDataAccess).GetMethod("SummaryTransactionDaily");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }

        [Fact]
        public void SummaryRecoverTypeDaily_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(SummaryTransactionDataAccess).GetMethod("SummaryRecoverTypeDaily");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }
    }
}
