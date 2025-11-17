using Device.Fingerprint.Features.FPDevices.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.FPDevices.Services
{
    /// <summary>
    /// Unit tests for FPDeviceDataAccess class.
    /// Note: This class requires database connection through ObjectDbFactory and DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class FPDeviceDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldCreateInstance()
        {
            // Act
            var dataAccess = new FPDeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IFPDeviceDataAccess>();
        }

        [Fact]
        public void FPDeviceDataAccess_ShouldImplementIFPDeviceDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new FPDeviceDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IFPDeviceDataAccess>();
        }

        [Fact]
        public void GetFPStagingTransIdForDevice_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(FPDeviceDataAccess).GetMethod("GetFPStagingTransIdForDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Contain("IEnumerable");
        }

        [Fact]
        public void UpdateDevice_MethodSignature_ShouldReturnDataResult()
        {
            // Arrange
            var method = typeof(FPDeviceDataAccess).GetMethod("UpdateDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Name.Should().Be("DataResult");
        }
    }
}
