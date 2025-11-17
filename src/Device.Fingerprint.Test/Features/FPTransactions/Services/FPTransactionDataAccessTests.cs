using Device.Fingerprint.Features.FPTransactions.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.FPTransactions.Services
{
    /// <summary>
    /// Unit tests for FPTransactionDataAccess class.
    /// Note: This class requires database connection through ObjectDbFactory and DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class FPTransactionDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new FPTransactionDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IFPTransactionDataAccess>();
        }

        [Fact]
        public void FPTransactionDataAccess_ShouldImplementIFPTransactionDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new FPTransactionDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IFPTransactionDataAccess>();
        }

        [Fact]
        public void GetFPRawTransaction_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(FPTransactionDataAccess).GetMethod("GetFPRawTransaction");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }

        [Fact]
        public void InsertTransaction_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(FPTransactionDataAccess).GetMethod("InsertTransaction");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }

        [Fact]
        public void InsertUserAgent_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(FPTransactionDataAccess).GetMethod("InsertUserAgent");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }

        [Fact]
        public void UnhandleSubscriberActionProcess_MethodSignature_ShouldReturnVoid()
        {
            // Arrange
            var method = typeof(FPTransactionDataAccess).GetMethod("UnhandleSubscriberActionProcess");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("Void");
        }
    }
}
