using Device.Fingerprint.Features.Devices.Model;
using Device.Fingerprint.Features.Devices.Services;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.Devices.Services
{
    public class DeviceServicesTests
    {
        private readonly Mock<IDeviceDataAccess> _mockDataAccess;
        private readonly DeviceServices _service;

        public DeviceServicesTests()
        {
            _mockDataAccess = new Mock<IDeviceDataAccess>();
            _service = new DeviceServices(_mockDataAccess.Object);
        }

        #region SearchDevice Tests

        [Fact]
        public void SearchDevice_WithValidParam_ShouldReturnDeviceList()
        {
            // Arrange
            var param = new SearchDeviceParamModel
            {
                DeviceCode = "DEV001",
                SubscriberID = 1
            };

            var expectedResult = new List<SearchDeviceItemModel>
            {
                new SearchDeviceItemModel 
                { 
                    DeviceID = 1,
                    DeviceCode = "DEV001", 
                    OS = "Windows" 
                },
                new SearchDeviceItemModel 
                { 
                    DeviceID = 2,
                    DeviceCode = "DEV002", 
                    OS = "iOS" 
                }
            };

            _mockDataAccess.Setup(x => x.SearchDevice(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.SearchDevice(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.SearchDevice(param), Times.Once);
        }

        [Fact]
        public void SearchDevice_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var param = new SearchDeviceParamModel
            {
                DeviceCode = "NONEXISTENT",
                SubscriberID = 1
            };

            var emptyResult = new List<SearchDeviceItemModel>();

            _mockDataAccess.Setup(x => x.SearchDevice(param))
                          .Returns(emptyResult);

            // Act
            var result = _service.SearchDevice(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockDataAccess.Verify(x => x.SearchDevice(param), Times.Once);
        }

        #endregion

        #region GetAssDeviceInfoByIDSubscriber Tests

        [Fact]
        public void GetAssDeviceInfoByIDSubscriber_WithValidParam_ShouldReturnDeviceInfo()
        {
            // Arrange
            var param = new GetAssDeviceInfoByIDSubscriberParamModel
            {
                DeviceID = 12345,
                SubscriberID = 1
            };

            var expectedResult = new GetAssDeviceInfoByIDSubscriberModel
            {
                AssociatedDevices = 5,
                AssociatedAccounts = 3
            };

            _mockDataAccess.Setup(x => x.GetAssDeviceInfoByIDSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssDeviceInfoByIDSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssDeviceInfoByIDSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssAccByDevSubscriber Tests

        [Fact]
        public void GetAssAccByDevSubscriber_WithValidParam_ShouldReturnAccountList()
        {
            // Arrange
            var param = new GetAssAccByDevSubscriberParamModel
            {
                DeviceID = 123,
                SubscriberID = 1,
                NoOfRecord = 25
            };

            var expectedResult = new List<GetAssAccByDevSubscriberItemModel>
            {
                new GetAssAccByDevSubscriberItemModel 
                { 
                    AccountID = 1001,
                    UserName = "user1",
                    RegisterName = "User One",
                    DeviceInCommon = 2,
                    OtherDeives = 1,
                    AssociatedAccount = 3
                },
                new GetAssAccByDevSubscriberItemModel 
                { 
                    AccountID = 1002,
                    UserName = "user2",
                    RegisterName = "User Two",
                    DeviceInCommon = 1,
                    OtherDeives = 2,
                    AssociatedAccount = 2
                }
            };

            _mockDataAccess.Setup(x => x.GetAssAccByDevSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssAccByDevSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssAccByDevSubscriber(param), Times.Once);
        }

        [Fact]
        public void GetAssAccByDevSubscriber_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var param = new GetAssAccByDevSubscriberParamModel
            {
                DeviceID = 999,
                SubscriberID = 999,
                NoOfRecord = 10
            };

            var emptyResult = new List<GetAssAccByDevSubscriberItemModel>();

            _mockDataAccess.Setup(x => x.GetAssAccByDevSubscriber(param))
                          .Returns(emptyResult);

            // Act
            var result = _service.GetAssAccByDevSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockDataAccess.Verify(x => x.GetAssAccByDevSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssDevByDevSubscriber Tests

        [Fact]
        public void GetAssDevByDevSubscriber_WithValidParam_ShouldReturnDeviceList()
        {
            // Arrange
            var param = new GetAssDevByDevSubscriberParamModel
            {
                DeviceID = 123,
                SubscriberID = 1,
                NoOfRecord = 10
            };

            var expectedResult = new List<GetAssDevByDevSubscriberItemModel>
            {
                new GetAssDevByDevSubscriberItemModel 
                { 
                    DeviceID = 123,
                    DeviceCode = "DEV001",
                    FirstSeenDate = DateTime.Now.AddDays(-10),
                    AssociatedAccounts = 3,
                    AssociatedDevices = 2
                },
                new GetAssDevByDevSubscriberItemModel 
                { 
                    DeviceID = 124,
                    DeviceCode = "DEV002",
                    FirstSeenDate = DateTime.Now.AddDays(-5),
                    AssociatedAccounts = 2,
                    AssociatedDevices = 1
                }
            };

            _mockDataAccess.Setup(x => x.GetAssDevByDevSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssDevByDevSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssDevByDevSubscriber(param), Times.Once);
        }

        #endregion

        #region GetAssMatrixByDeviceSubscriber Tests

        [Fact]
        public void GetAssMatrixByDeviceSubscriber_WithValidParam_ShouldReturnMatrixList()
        {
            // Arrange
            var param = new GetAssMatrixByDeviceSubscriberParamModel
            {
                DeviceID = 123,
                SubscriberID = 1,
                NoOfCustomer = 50,
                NoOfDevice = 100
            };

            var expectedResult = new List<GetAssMatrixByDeviceSubscriberItemModel>
            {
                new GetAssMatrixByDeviceSubscriberItemModel 
                { 
                    AccountID = 1001,
                    UserName = "user1",
                    RegisterName = "User One",
                    DeviceID = 123,
                    FirstDeviceCode = "DEV001",
                    CreatedTime = DateTime.Now.AddDays(-30)
                }
            };

            _mockDataAccess.Setup(x => x.GetAssMatrixByDeviceSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssMatrixByDeviceSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.GetAssMatrixByDeviceSubscriber(param), Times.Once);
        }

        #endregion

        #region GetDevicesPerAccount Tests

        [Fact]
        public void GetDevicesPerAccount_WithValidParam_ShouldReturnDevicesPerAccountResult()
        {
            // Arrange
            var param = new GetDevicesPerAccountParamModel
            {
                PageSize = 50,
                PageIndex = 1,
                SubscriberId = 1,
                TimeRange = 30,
                NoOfDevice = 10
            };

            var expectedResult = new GetDevicesPerAccountResultModel
            {
                TotalRows = 2,
                Items = new List<GetDevicesPerAccountItemModel>
                {
                    new GetDevicesPerAccountItemModel 
                    { 
                        CTSCustID = 1001,
                        UserName = "user1",
                        RegisterName = "User One",
                        NoOfDevice = 3
                    },
                    new GetDevicesPerAccountItemModel 
                    { 
                        CTSCustID = 1002,
                        UserName = "user2",
                        RegisterName = "User Two",
                        NoOfDevice = 2
                    }
                }
            };

            _mockDataAccess.Setup(x => x.GetDevicesPerAccount(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetDevicesPerAccount(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResult);
            _mockDataAccess.Verify(x => x.GetDevicesPerAccount(param), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DeviceServices(null));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void SearchDevice_WithNullParam_ShouldPassToDataAccess()
        {
            // Arrange
            SearchDeviceParamModel param = null;
            var expectedResult = new List<SearchDeviceItemModel>();

            _mockDataAccess.Setup(x => x.SearchDevice(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.SearchDevice(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockDataAccess.Verify(x => x.SearchDevice(param), Times.Once);
        }

        [Fact]
        public void GetAssDeviceInfoByIDSubscriber_WithNullParam_ShouldPassToDataAccess()
        {
            // Arrange
            GetAssDeviceInfoByIDSubscriberParamModel param = null;
            var expectedResult = new GetAssDeviceInfoByIDSubscriberModel();

            _mockDataAccess.Setup(x => x.GetAssDeviceInfoByIDSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetAssDeviceInfoByIDSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            _mockDataAccess.Verify(x => x.GetAssDeviceInfoByIDSubscriber(param), Times.Once);
        }

        [Fact]
        public void GetAssAccByDevSubscriber_WithNullParam_ShouldReturnEmptyCollection()
        {
            // Arrange
            GetAssAccByDevSubscriberParamModel param = null;
            var emptyResult = new List<GetAssAccByDevSubscriberItemModel>();

            _mockDataAccess.Setup(x => x.GetAssAccByDevSubscriber(param))
                          .Returns(emptyResult);

            // Act
            var result = _service.GetAssAccByDevSubscriber(param);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _mockDataAccess.Verify(x => x.GetAssAccByDevSubscriber(param), Times.Once);
        }

        #endregion
    }
}