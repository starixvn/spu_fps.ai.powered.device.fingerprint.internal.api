using Device.Fingerprint.Features.Transaction.Model;
using Device.Fingerprint.Features.Transaction.Services;
using Device.Fingerprint.Model;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.Transaction.Services
{
    public class TransactionServiceTests
    {
        private readonly Mock<ITransactionDataAccess> _mockDataAccess;
        private readonly TransactionService _service;

        public TransactionServiceTests()
        {
            _mockDataAccess = new Mock<ITransactionDataAccess>();
            _service = new TransactionService(_mockDataAccess.Object);
        }

        #region InsertRawTransaction Tests

        [Fact]
        public void InsertRawTransaction_WithValidTransactions_ShouldCallDataAccess()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel
                {
                    Action = "LOGIN",
                    ActionResult = "SUCCESS",
                    DeviceCode = "DEV001",
                    IP = "192.168.1.1",
                    LoginName = "testuser",
                    SubscriberName = "testsubscriber",
                    TransTime = DateTime.Now,
                    UserAgent = "Mozilla/5.0"
                }
            };
            var isTransOfDBExtra = false;

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), isTransOfDBExtra))
                          .Verifiable();

            // Act
            _service.InsertRawTransaction(transactions, isTransOfDBExtra);

            // Assert
            _mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>(), isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void InsertRawTransaction_WithEmptyTransactions_ShouldStillCallDataAccess()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>();
            var isTransOfDBExtra = true;

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), isTransOfDBExtra))
                          .Verifiable();

            // Act
            _service.InsertRawTransaction(transactions, isTransOfDBExtra);

            // Assert
            _mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>(), isTransOfDBExtra), Times.Once);
        }

        #endregion

        #region InsertFPTagging Tests

        [Fact]
        public void InsertFPTagging_WithValidTaggings_ShouldCallDataAccess()
        {
            // Arrange
            var taggings = "{\"tag1\":\"value1\"}";
            var isTransOfDBExtra = false;

            _mockDataAccess.Setup(x => x.InsertFPTagging(taggings, isTransOfDBExtra))
                          .Verifiable();

            // Act
            _service.InsertFPTagging(taggings, isTransOfDBExtra);

            // Assert
            _mockDataAccess.Verify(x => x.InsertFPTagging(taggings, isTransOfDBExtra), Times.Once);
        }

        #endregion

        #region InsertTransaction Tests

        [Fact]
        public void InsertTransaction_WithValidTransactions_ShouldCallDataAccess()
        {
            // Arrange
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    LoginName = "testuser",
                    SubscriberId = 1,
                    TransTime = DateTime.Now,
                    CustId = 100,
                    Action = "LOGIN"
                }
            };

            _mockDataAccess.Setup(x => x.InsertTransaction(It.IsAny<string>()))
                          .Verifiable();

            // Act
            _service.InsertTransaction(transactions);

            // Assert
            _mockDataAccess.Verify(x => x.InsertTransaction(It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region DeviceIdentify Tests

        [Fact]
        public void DeviceIdentify_WithValidParameters_ShouldCallDataAccess()
        {
            // Arrange
            var transJson = "{\"transactions\":[]}";
            var groupType = 1;

            _mockDataAccess.Setup(x => x.DeviceIdentify(transJson, groupType))
                          .Verifiable();

            // Act
            _service.DeviceIdentify(transJson, groupType);

            // Assert
            _mockDataAccess.Verify(x => x.DeviceIdentify(transJson, groupType), Times.Once);
        }

        [Fact]
        public void DeviceIdentifyNoFingerprint_WithValidRawTransIds_ShouldCallDataAccess()
        {
            // Arrange
            var rawTransIds = "1,2,3,4,5";

            _mockDataAccess.Setup(x => x.DeviceIdentifyNoFingerprint(rawTransIds))
                          .Verifiable();

            // Act
            _service.DeviceIdentifyNoFingerprint(rawTransIds);

            // Assert
            _mockDataAccess.Verify(x => x.DeviceIdentifyNoFingerprint(rawTransIds), Times.Once);
        }

        #endregion

        #region GetTransactionHistory Tests

        [Fact]
        public void GetTransactionHistory_WithCTSCustID_ShouldCallByCustSubscriber()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                CTSCustID = 123,
                SubscriberID = 1
            };

            var expectedResult = new TransactionResultModel();

            _mockDataAccess.Setup(x => x.GetTransactionByCustSubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetTransactionHistory(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetTransactionByCustSubscriber(param), Times.Once);
            _mockDataAccess.Verify(x => x.GetTransactionBySubscriber(It.IsAny<TransactionParamModel>()), Times.Never);
        }

        [Fact]
        public void GetTransactionHistory_WithoutCTSCustID_ShouldCallBySubscriber()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                CTSCustID = null,
                SubscriberID = 1
            };

            var expectedResult = new TransactionResultModel();

            _mockDataAccess.Setup(x => x.GetTransactionBySubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetTransactionHistory(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetTransactionBySubscriber(param), Times.Once);
            _mockDataAccess.Verify(x => x.GetTransactionByCustSubscriber(It.IsAny<TransactionParamModel>()), Times.Never);
        }

        [Fact]
        public void GetTransactionHistory_WithZeroCTSCustID_ShouldCallBySubscriber()
        {
            // Arrange
            var param = new TransactionParamModel
            {
                CTSCustID = 0,
                SubscriberID = 1
            };

            var expectedResult = new TransactionResultModel();

            _mockDataAccess.Setup(x => x.GetTransactionBySubscriber(param))
                          .Returns(expectedResult);

            // Act
            var result = _service.GetTransactionHistory(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            _mockDataAccess.Verify(x => x.GetTransactionBySubscriber(param), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new TransactionService(null));
        }

        #endregion

        #region Simple Integration Tests

        [Fact]
        public void MergeTransaction_WithValidJson_ShouldCallDataAccess()
        {
            // Arrange
            var transJson = "{\"merge\":\"data\"}";

            _mockDataAccess.Setup(x => x.UpdateMergeTransaction(transJson))
                          .Verifiable();

            // Act
            _service.MergeTransaction(transJson);

            // Assert
            _mockDataAccess.Verify(x => x.UpdateMergeTransaction(transJson), Times.Once);
        }

        [Fact]
        public void ForceMergeTransaction_WithValidJson_ShouldCallDataAccess()
        {
            // Arrange
            var transJson = "{\"forceMerge\":\"data\"}";

            _mockDataAccess.Setup(x => x.UpdateForceMergeTransaction(transJson))
                          .Verifiable();

            // Act
            _service.ForceMergeTransaction(transJson);

            // Assert
            _mockDataAccess.Verify(x => x.UpdateForceMergeTransaction(transJson), Times.Once);
        }

        [Fact]
        public void EvolutionNewDevice_WithValidJson_ShouldCallDataAccess()
        {
            // Arrange
            var transJson = "{\"evolution\":\"newDevice\"}";

            _mockDataAccess.Setup(x => x.EvolutionNewDevice(transJson))
                          .Verifiable();

            // Act
            _service.EvolutionNewDevice(transJson);

            // Assert
            _mockDataAccess.Verify(x => x.EvolutionNewDevice(transJson), Times.Once);
        }

        [Fact]
        public void EvolutionRecoverDevice_WithValidJson_ShouldCallDataAccess()
        {
            // Arrange
            var transJson = "{\"evolution\":\"recoverDevice\"}";

            _mockDataAccess.Setup(x => x.EvolutionRecoverDevice(transJson))
                          .Verifiable();

            // Act
            _service.EvolutionRecoverDevice(transJson);

            // Assert
            _mockDataAccess.Verify(x => x.EvolutionRecoverDevice(transJson), Times.Once);
        }

        #endregion
    }
}