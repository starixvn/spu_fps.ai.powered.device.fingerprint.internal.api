using Device.Fingerprint.Features.Transaction.Model;
using Device.Fingerprint.Features.Transaction.Services;
using Device.Fingerprint.Model;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;

namespace Device.Fingerprint.Test.Features.Transaction.Services
{
    /// <summary>
    /// Additional tests focusing on data transformation for TransactionService
    /// </summary>
    public class TransactionServiceDataTransformationTests
    {
        private readonly Mock<ITransactionDataAccess> _mockDataAccess;
        private readonly TransactionService _service;

        public TransactionServiceDataTransformationTests()
        {
            _mockDataAccess = new Mock<ITransactionDataAccess>();
            _service = new TransactionService(_mockDataAccess.Object);
        }

        #region JSON Serialization Tests

        [Fact]
        public void InsertTransaction_ShouldSerializeTransactionEntitiesToJson()
        {
            // Arrange
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    LoginName = "testuser",
                    SubscriberId = 1,
                    TransTime = new DateTime(2024, 1, 15),
                    CustId = 100,
                    Action = "LOGIN",
                    ClientIP = "192.168.1.1"
                }
            };

            string capturedJson = null;

            _mockDataAccess.Setup(x => x.InsertTransaction(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            _service.InsertTransaction(transactions);

            // Assert
            capturedJson.Should().NotBeNull();
            
            // Verify JSON can be deserialized back to original structure
            var deserializedTransactions = JsonConvert.DeserializeObject<List<TransactionEntity>>(capturedJson);
            deserializedTransactions.Should().HaveCount(1);
            deserializedTransactions.First().LoginName.Should().Be("testuser");
            deserializedTransactions.First().Action.Should().Be("LOGIN");
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public void InsertRawTransaction_WithEmptyList_ShouldCreateEmptyJsonArray()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>();
            string capturedJson = null;

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), It.IsAny<bool>()))
                          .Callback<string, bool>((json, isExtra) => capturedJson = json);

            // Act
            _service.InsertRawTransaction(transactions, false);

            // Assert
            capturedJson.Should().NotBeNull();
            capturedJson.Should().Be("[]");
        }

        [Fact]
        public void InsertTransaction_WithEmptyList_ShouldCreateEmptyJsonArray()
        {
            // Arrange
            var transactions = new List<TransactionEntity>();
            string capturedJson = null;

            _mockDataAccess.Setup(x => x.InsertTransaction(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            _service.InsertTransaction(transactions);

            // Assert
            capturedJson.Should().NotBeNull();
            capturedJson.Should().Be("[]");
        }

        [Fact]
        public void InsertRawTransaction_WithSpecialCharacters_ShouldEscapeCorrectly()
        {
            // Arrange
            var transaction = new RawTransactionModel
            {
                Action = "Special \"quoted\" action",
                LoginName = "user@domain.com",
                UserAgent = "Mozilla/5.0 (Windows; U; \"test\")",
                URL = "https://example.com/path?param=\"value\"&other=test"
            };

            var transactions = new List<RawTransactionModel> { transaction };
            string capturedJson = null;

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), It.IsAny<bool>()))
                          .Callback<string, bool>((json, isExtra) => capturedJson = json);

            // Act
            _service.InsertRawTransaction(transactions, false);

            // Assert
            capturedJson.Should().NotBeNull();
            
            // Verify the JSON is valid and can be deserialized
            Action deserializeAction = () => JsonConvert.DeserializeObject(capturedJson);
            deserializeAction.Should().NotThrow();
        }

        #endregion

        #region Performance and Large Data Tests

        [Fact]
        public void InsertRawTransaction_WithLargeDataset_ShouldHandleEfficiently()
        {
            // Arrange
            var transactions = Enumerable.Range(1, 100) // Reduced size for faster test
                .Select(i => new RawTransactionModel
                {
                    Action = $"ACTION_{i}",
                    DeviceCode = $"DEV_{i:D6}",
                    LoginName = $"user{i}@example.com",
                    TransTime = DateTime.Now.AddMinutes(-i)
                })
                .ToList();

            string capturedJson = null;

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), It.IsAny<bool>()))
                          .Callback<string, bool>((json, isExtra) => capturedJson = json);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _service.InsertRawTransaction(transactions, false);
            stopwatch.Stop();

            // Assert
            capturedJson.Should().NotBeNull();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
            
            // Verify JSON structure is correct
            Action deserializeAction = () => JsonConvert.DeserializeObject(capturedJson);
            deserializeAction.Should().NotThrow();
        }

        #endregion

        #region Simple Integration Tests

        [Fact]
        public void InsertRawTransaction_WithValidData_ShouldCallDataAccess()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel
                {
                    Action = "LOGIN",
                    DeviceCode = "DEV001",
                    LoginName = "user1",
                    IP = "192.168.1.1"
                }
            };

            _mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>(), It.IsAny<bool>()))
                          .Verifiable();

            // Act
            _service.InsertRawTransaction(transactions, false);

            // Assert
            _mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>(), false), Times.Once);
        }

        [Fact]
        public void InsertTransaction_WithValidData_ShouldCallDataAccess()
        {
            // Arrange
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity
                {
                    LoginName = "testuser",
                    Action = "PAYMENT",
                    ClientIP = "10.0.0.1"
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
    }
}