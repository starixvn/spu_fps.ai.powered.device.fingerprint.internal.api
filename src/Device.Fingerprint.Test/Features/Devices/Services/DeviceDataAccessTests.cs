using Device.Fingerprint.Features.Devices.Services;
using FluentAssertions;

namespace Device.Fingerprint.Test.Features.Devices.Services
{
    /// <summary>
    /// Unit tests for DeviceDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation tests. For full testing, integration tests
    /// with database are recommended.
    /// </summary>
    public class DeviceDataAccessTests
    {
        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IDeviceDataAccess>();
        }

        [Fact]
        public void DeviceDataAccess_ShouldImplementIDeviceDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IDeviceDataAccess>();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveSearchDeviceMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("SearchDevice").Should().NotBeNull();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveGetAssDeviceInfoByIDSubscriberMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("GetAssDeviceInfoByIDSubscriber").Should().NotBeNull();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveGetAssAccByDevSubscriberMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("GetAssAccByDevSubscriber").Should().NotBeNull();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveGetAssDevByDevSubscriberMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("GetAssDevByDevSubscriber").Should().NotBeNull();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveGetAssMatrixByDeviceSubscriberMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("GetAssMatrixByDeviceSubscriber").Should().NotBeNull();
        }

        [Fact]
        public void DeviceDataAccess_ShouldHaveGetDevicesPerAccountMethod()
        {
            // Arrange
            var dataAccess = new DeviceDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            typeof(DeviceDataAccess).GetMethod("GetDevicesPerAccount").Should().NotBeNull();
        }
    }
}
