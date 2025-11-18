using Device.Fingerprint.Features.Account.Model;
using Device.Fingerprint.Features.Account.Services;
using FluentAssertions;
using System.Reflection;

namespace Device.Fingerprint.Test.Features.Account.Services
{
    /// <summary>
    /// Unit tests for AccountDataAccess class.
    /// Note: This class requires database connection through DynamicRepository,
    /// so these are basic instantiation and method signature tests. 
    /// For full testing, integration tests with database are recommended.
    /// </summary>
    public class AccountDataAccessTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_ShouldInitializeRepository()
        {
            // Act
            var dataAccess = new AccountDataAccess();

            // Assert
            dataAccess.Should().NotBeNull();
            dataAccess.Should().BeAssignableTo<IAccountDataAccess>();
        }

        [Fact]
        public void AccountDataAccess_ShouldImplementIAccountDataAccessInterface()
        {
            // Arrange & Act
            var dataAccess = new AccountDataAccess();

            // Assert
            dataAccess.Should().BeAssignableTo<IAccountDataAccess>();
        }

        #endregion

        #region SearchPartialAccount Tests

        [Fact]
        public void SearchPartialAccount_MethodSignature_ShouldReturnSearchAccountResultModel()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("SearchPartialAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(SearchAccountResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(SearchAccountParamModel));
        }

        [Fact]
        public void SearchPartialAccount_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("SearchPartialAccount");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region SearchExactAccount Tests

        [Fact]
        public void SearchExactAccount_MethodSignature_ShouldReturnSearchAccountResultModel()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("SearchExactAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(SearchAccountResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(SearchAccountParamModel));
        }

        [Fact]
        public void SearchExactAccount_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("SearchExactAccount");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssAccountInfoByCustSubscriber Tests

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssAccountInfoByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssAccountInfoByCustSubscriberModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssAccountInfoByCustSubscriberParamModel));
        }

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssAccountInfoByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssAccountsByCustSubscriber Tests

        [Fact]
        public void GetAssAccountsByCustSubscriber_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssAccountsByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssAccountsByCustSubscriberResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssAccountsByCustSubscriberParamModel));
        }

        [Fact]
        public void GetAssAccountsByCustSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssAccountsByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssDevicesByCustSubscriber Tests

        [Fact]
        public void GetAssDevicesByCustSubscriber_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssDevicesByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssDevicesByCustSubscriberResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssDevicesByCustSubscriberParamModel));
        }

        [Fact]
        public void GetAssDevicesByCustSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssDevicesByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAssMatrixAccountByCustSubscriber Tests

        [Fact]
        public void GetAssMatrixAccountByCustSubscriber_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssMatrixAccountByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssMatrixAccountByCustSubscriberResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAssMatrixAccountByCustSubscriberParamModel));
        }

        [Fact]
        public void GetAssMatrixAccountByCustSubscriber_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAssMatrixAccountByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region GetAccountsPerDevice Tests

        [Fact]
        public void GetAccountsPerDevice_MethodSignature_ShouldHaveCorrectReturnType()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAccountsPerDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAccountsPerDeviceResultModel));
            method.GetParameters().Should().HaveCount(1);
            method.GetParameters()[0].ParameterType.Should().Be(typeof(GetAccountPerDeviceParamModel));
        }

        [Fact]
        public void GetAccountsPerDevice_MethodSignature_ShouldBePublic()
        {
            // Arrange
            var method = typeof(AccountDataAccess).GetMethod("GetAccountsPerDevice");

            // Assert
            method.Should().NotBeNull();
            method!.IsPublic.Should().BeTrue();
        }

        #endregion

        #region Interface Tests

        [Fact]
        public void IAccountDataAccess_ShouldHaveSearchPartialAccountMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("SearchPartialAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(SearchAccountResultModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveSearchExactAccountMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("SearchExactAccount");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(SearchAccountResultModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveGetAssAccountInfoByCustSubscriberMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("GetAssAccountInfoByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssAccountInfoByCustSubscriberModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveGetAssAccountsByCustSubscriberMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("GetAssAccountsByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssAccountsByCustSubscriberResultModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveGetAssDevicesByCustSubscriberMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("GetAssDevicesByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssDevicesByCustSubscriberResultModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveGetAssMatrixAccountByCustSubscriberMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("GetAssMatrixAccountByCustSubscriber");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAssMatrixAccountByCustSubscriberResultModel));
        }

        [Fact]
        public void IAccountDataAccess_ShouldHaveGetAccountsPerDeviceMethod()
        {
            // Arrange
            var method = typeof(IAccountDataAccess).GetMethod("GetAccountsPerDevice");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(GetAccountsPerDeviceResultModel));
        }

        #endregion

        #region Repository Tests

        [Fact]
        public void AccountDataAccess_ShouldHavePrivateRepositoryField()
        {
            // Arrange
            var field = typeof(AccountDataAccess).GetField("repository", BindingFlags.NonPublic | BindingFlags.Instance);

            // Assert
            field.Should().NotBeNull();
            field!.FieldType.Name.Should().Be("IDynamicRepository");
        }

        #endregion

        #region Method Count Tests

        [Fact]
        public void IAccountDataAccess_ShouldHave7Methods()
        {
            // Arrange
            var methods = typeof(IAccountDataAccess).GetMethods();

            // Assert
            methods.Should().HaveCount(7, "interface should have 7 methods");
        }

        [Fact]
        public void AccountDataAccess_ShouldImplementAll7InterfaceMethods()
        {
            // Arrange
            var interfaceMethods = typeof(IAccountDataAccess).GetMethods();
            var classMethods = typeof(AccountDataAccess).GetMethods()
                .Where(m => interfaceMethods.Any(im => im.Name == m.Name));

            // Assert
            classMethods.Should().HaveCount(7, "class should implement all 7 interface methods");
        }

        #endregion
    }
}
