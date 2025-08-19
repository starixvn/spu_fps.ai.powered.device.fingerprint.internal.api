using Device.Fingerprint.Features.Account.Model;
using Device.Fingerprint.Features.Account.Services;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.Account.Services
{
    public class AccountServicesTests
    {
        private readonly Mock<IAccountDataAccess> _mockDataAccess;
        private readonly AccountServices _service;

        public AccountServicesTests()
        {
            _mockDataAccess = new Mock<IAccountDataAccess>();
            _service = new AccountServices(_mockDataAccess.Object);
        }

        #region SearchAccount Tests

        [Fact]
        public void SearchAccount_WithPartialSearch_ShouldCallSearchPartialAccount()
        {
            // Arrange
            var param = new SearchAccountParamModel
            {
                IsSearchPartial = true,
                UserName = "test",
                SubscriberID = 1,
                Take = 10,
                Skip = 0
            };

            var expectedResult = new SearchAccountResultModel();

            _mockDataAccess.Setup(x => x.SearchPartialAccount(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.SearchAccount(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.SearchPartialAccount(param), Times.Once);
            _mockDataAccess.Verify(x => x.SearchExactAccount(It.IsAny<SearchAccountParamModel>()), Times.Never);
        }

        [Fact]
        public void SearchAccount_WithExactSearch_ShouldCallSearchExactAccount()
        {
            // Arrange
            var param = new SearchAccountParamModel
            {
                IsSearchPartial = false,
                UserName = "testexact",
                SubscriberID = 1
            };

            var expectedResult = new SearchAccountResultModel();

            _mockDataAccess.Setup(x => x.SearchExactAccount(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.SearchAccount(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.SearchExactAccount(param), Times.Once);
            _mockDataAccess.Verify(x => x.SearchPartialAccount(It.IsAny<SearchAccountParamModel>()), Times.Never);
        }

        #endregion

        #region GetAssAccountInfoByCustSubscriber Tests

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_WithValidParam_ShouldReturnAccountInfo()
        {
            // Arrange
            var param = new GetAssAccountInfoByCustSubscriberParamModel
            {
                SubscriberID = 1
            };

            var expectedResult = new GetAssAccountInfoByCustSubscriberModel();

            _mockDataAccess.Setup(x => x.GetAssAccountInfoByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssAccountInfoByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssAccountInfoByCustSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssAccountsByCustSubscriber Tests

        [Fact]
        public void GetAssAccountsByCustSubscriber_WithValidParam_ShouldReturnAccountsResult()
        {
            // Arrange
            var param = new GetAssAccountsByCustSubscriberParamModel
            {
                SubscriberID = 2
            };

            var expectedResult = new GetAssAccountsByCustSubscriberResultModel();

            _mockDataAccess.Setup(x => x.GetAssAccountsByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssAccountsByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssAccountsByCustSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssDevicesByCustSubscriber Tests

        [Fact]
        public void GetAssDevicesByCustSubscriber_WithValidParam_ShouldReturnDevicesResult()
        {
            // Arrange
            var param = new GetAssDevicesByCustSubscriberParamModel
            {
                SubscriberID = 3
            };

            var expectedResult = new GetAssDevicesByCustSubscriberResultModel();

            _mockDataAccess.Setup(x => x.GetAssDevicesByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssDevicesByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssDevicesByCustSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssMatrixAccountByCustSubscriber Tests

        [Fact]
        public void GetAssMatrixAccountByCustSubscriber_WithValidParam_ShouldReturnMatrixResult()
        {
            // Arrange
            var param = new GetAssMatrixAccountByCustSubscriberParamModel
            {
                SubscriberID = 4
            };

            var expectedResult = new GetAssMatrixAccountByCustSubscriberResultModel();

            _mockDataAccess.Setup(x => x.GetAssMatrixAccountByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssMatrixAccountByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssMatrixAccountByCustSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAccountsPerDevice Tests

        [Fact]
        public void GetAccountsPerDevice_WithValidParam_ShouldReturnAccountsPerDeviceResult()
        {
            // Arrange
            var param = new GetAccountPerDeviceParamModel
            {
                SubscriberId = 5
            };

            var expectedResult = new GetAccountsPerDeviceResultModel();

            _mockDataAccess.Setup(x => x.GetAccountsPerDevice(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAccountsPerDevice(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAccountsPerDevice(param), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AccountServices(null));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void SearchAccount_WithNullParam_ShouldPassToDataAccess()
        {
            // Arrange
            SearchAccountParamModel param = null;
            var expectedResult = new SearchAccountResultModel();

            _mockDataAccess.Setup(x => x.SearchExactAccount(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.SearchAccount(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.SearchExactAccount(param), Times.Once);
        }

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_WithNullParam_ShouldPassToDataAccess()
        {
            // Arrange
            GetAssAccountInfoByCustSubscriberParamModel param = null;
            var expectedResult = new GetAssAccountInfoByCustSubscriberModel();

            _mockDataAccess.Setup(x => x.GetAssAccountInfoByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssAccountInfoByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssAccountInfoByCustSubscriber(param), Times.Once);
        }

        #endregion
    }
}