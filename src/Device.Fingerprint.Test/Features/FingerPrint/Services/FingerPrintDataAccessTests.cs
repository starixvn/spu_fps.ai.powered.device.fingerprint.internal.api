using Device.Fingerprint.Features.FingerPrint.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.FingerPrint.Services
{
    /// <summary>
    /// Unit tests for FingerPrintDataAccess class.
    /// Note: This class requires database connection through ObjectDbFactory,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class FingerPrintDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Act
            var dataAccess = new FingerPrintDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IFingerPrintDataAccess>();
        }

        [Fact]
        public void FingerPrintDataAccess_ShouldImplementIFingerPrintDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new FingerPrintDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IFingerPrintDataAccess>();
        }

        [Fact]
        public void GetFPFingerPrint_MethodSignature_ShouldReturnIEnumerableOfFPFingerPrintEntity()
        {
            // Arrange
            var method = typeof(FingerPrintDataAccess).GetMethod("GetFPFingerPrint");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }
    }
}
