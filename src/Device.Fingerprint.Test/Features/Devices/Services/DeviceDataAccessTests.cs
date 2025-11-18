using Device.Fingerprint.Features.Devices.Model;
using Device.Fingerprint.Features.Devices.Services;
using FluentAssertions;
using System.Reflection;

namespace Device.Fingerprint.Test.Features.Devices.Services
{
    /// <summary>
    /// Unit tests for DeviceDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation and method signature tests. 
    /// For full testing, integration tests with database are recommended.
    /// </summary>
    public class DeviceDataAccessTests
    {
        #region Constructor Tests

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

        #endregion

        #region SearchDevice Tests

        [Fact]
        public void SearchDevice_MethodSignature_ShouldReturnIEnumerableOfSearchDeviceItemModel()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("SearchDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<SearchDeviceItemModel>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(SearchDeviceParamModel));
        }

        [Fact]
        public void SearchDevice_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("SearchDevice");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssDeviceInfoByIDSubscriber Tests

        [Fact]
        public void GetAssDeviceInfoByIDSubscriber_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssDeviceInfoByIDSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssDeviceInfoByIDSubscriberModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssDeviceInfoByIDSubscriberParamModel));
        }

        [Fact]
        public void GetAssDeviceInfoByIDSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssDeviceInfoByIDSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssAccByDevSubscriber Tests

        [Fact]
        public void GetAssAccByDevSubscriber_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssAccByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssAccByDevSubscriberItemModel>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssAccByDevSubscriberParamModel));
        }

        [Fact]
        public void GetAssAccByDevSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssAccByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssDevByDevSubscriber Tests

        [Fact]
        public void GetAssDevByDevSubscriber_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssDevByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssDevByDevSubscriberItemModel>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssDevByDevSubscriberParamModel));
        }

        [Fact]
        public void GetAssDevByDevSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssDevByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssMatrixByDeviceSubscriber Tests

        [Fact]
        public void GetAssMatrixByDeviceSubscriber_MethodSignature_ShouldReturnIEnumerable()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssMatrixByDeviceSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssMatrixByDeviceSubscriberItemModel>));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssMatrixByDeviceSubscriberParamModel));
        }

        [Fact]
        public void GetAssMatrixByDeviceSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetAssMatrixByDeviceSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetDevicesPerAccount Tests

        [Fact]
        public void GetDevicesPerAccount_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetDevicesPerAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetDevicesPerAccountResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetDevicesPerAccountParamModel));
        }

        [Fact]
        public void GetDevicesPerAccount_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(DeviceDataAccess).GetMethod("GetDevicesPerAccount");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region Interface Tests

        [Fact]
        public void IDeviceDataAccess_ShouldHaveSearchDeviceMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("SearchDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<SearchDeviceItemModel>));
        }

        [Fact]
        public void IDeviceDataAccess_ShouldHaveGetAssDeviceInfoByIDSubscriberMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("GetAssDeviceInfoByIDSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssDeviceInfoByIDSubscriberModel));
        }

        [Fact]
        public void IDeviceDataAccess_ShouldHaveGetAssAccByDevSubscriberMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("GetAssAccByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssAccByDevSubscriberItemModel>));
        }

        [Fact]
        public void IDeviceDataAccess_ShouldHaveGetAssDevByDevSubscriberMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("GetAssDevByDevSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssDevByDevSubscriberItemModel>));
        }

        [Fact]
        public void IDeviceDataAccess_ShouldHaveGetAssMatrixByDeviceSubscriberMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("GetAssMatrixByDeviceSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().BeAssignableTo(typeof(IEnumerable<GetAssMatrixByDeviceSubscriberItemModel>));
        }

        [Fact]
        public void IDeviceDataAccess_ShouldHaveGetDevicesPerAccountMethod()
        {
            // Arrange
            var method = typeof(IDeviceDataAccess).GetMethod("GetDevicesPerAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetDevicesPerAccountResultModel));
        }

        #endregion

        #region Repository Tests

        [Fact]
        public void DeviceDataAccess_ShouldHavePrivateRepositoryField()
        {
            // Arrange
            var field = typeof(DeviceDataAccess).GetField("repository", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            field.Should().NotBeNull();
            field!.FieldType.Name.Should().Be("IDynamicRepository");
        }

        #endregion

        #region Method Count Tests

        [Fact]
        public void IDeviceDataAccess_ShouldHave6Methods()
        {
            // Arrange
            var methods = typeof(IDeviceDataAccess).GetMethods();

            // Assert
            methods.Should().HaveCount(6, "interface should have 6 methods");
        }

        [Fact]
        public void DeviceDataAccess_ShouldImplementAll6InterfaceMethods()
        {
            // Arrange
            var interfaceMethods = typeof(IDeviceDataAccess).GetMethods();
            var classMethods = typeof(DeviceDataAccess).GetMethods()
                .Where(m => interfaceMethods.Any(im => im.Name == m.Name));

            // Assert
            classMethods.Should().HaveCount(6, "class should implement all 6 interface methods");
        }

        #endregion
    }
}
