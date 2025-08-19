using Device.Fingerprint.Features.Subscriber.Services;
using Device.Fingerprint.Model;
using Device.Fingerprint.Features.Subscriber.Model;
using FluentAssertions;
using Moq;

namespace Device.Fingerprint.Test.Features.Subscriber.Services
{
    public class SubscriberServicesTests
    {
        private readonly Mock<ISubscriberDataAccess> _mockDataAccess;
        private readonly SubscriberServices _service;

        public SubscriberServicesTests()
        {
            _mockDataAccess = new Mock<ISubscriberDataAccess>();
            _service = new SubscriberServices(_mockDataAccess.Object);
        }

        #region GetSubscriberByName Tests

        [Fact]
        public void GetSubscriberByName_WithExistingSubscriber_ShouldReturnSubscriberModel()
        {
            // Arrange
            var subscriberName = "TestSubscriber";
            var subscriberEntity = new SubscriberEntity
            {
                SubscriberId = 1,
                SubscriberName = subscriberName
            };

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns(subscriberEntity);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().NotBeNull();
            result.SubscriberId.Should().Be(1);
            result.SubscriberName.Should().Be(subscriberName);

            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        [Fact]
        public void GetSubscriberByName_WithNonExistentSubscriber_ShouldReturnNull()
        {
            // Arrange
            var subscriberName = "NonExistentSubscriber";

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns((SubscriberEntity)null);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().BeNull();

            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        [Fact]
        public void GetSubscriberByName_WithEmptyString_ShouldCallDataAccess()
        {
            // Arrange
            var subscriberName = "";

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns((SubscriberEntity)null);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().BeNull();
            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        [Fact]
        public void GetSubscriberByName_WithNullString_ShouldCallDataAccess()
        {
            // Arrange
            string subscriberName = null;

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns((SubscriberEntity)null);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().BeNull();
            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        #endregion

        #region GetSubscriberList Tests

        [Fact]
        public void GetSubscriberList_WithExistingSubscribers_ShouldReturnSubscriberModelList()
        {
            // Arrange
            var subscriberEntities = new List<SubscriberEntity>
            {
                new SubscriberEntity { SubscriberId = 1, SubscriberName = "Subscriber1" },
                new SubscriberEntity { SubscriberId = 2, SubscriberName = "Subscriber2" },
                new SubscriberEntity { SubscriberId = 3, SubscriberName = "Subscriber3" }
            };

            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns(subscriberEntities);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var resultList = result.ToList();
            resultList[0].SubscriberId.Should().Be(1);
            resultList[0].SubscriberName.Should().Be("Subscriber1");
            resultList[1].SubscriberId.Should().Be(2);
            resultList[1].SubscriberName.Should().Be("Subscriber2");
            resultList[2].SubscriberId.Should().Be(3);
            resultList[2].SubscriberName.Should().Be("Subscriber3");

            _mockDataAccess.Verify(x => x.GetSubscriberList(), Times.Once);
        }

        [Fact]
        public void GetSubscriberList_WithEmptyResult_ShouldReturnEmptyList()
        {
            // Arrange
            var emptyList = new List<SubscriberEntity>();

            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns(emptyList);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockDataAccess.Verify(x => x.GetSubscriberList(), Times.Once);
        }

        [Fact]
        public void GetSubscriberList_WithNullResult_ShouldReturnEmptyList()
        {
            // Arrange
            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns((IEnumerable<SubscriberEntity>)null);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _mockDataAccess.Verify(x => x.GetSubscriberList(), Times.Once);
        }

        [Fact]
        public void GetSubscriberList_WithSingleSubscriber_ShouldReturnSingleItemList()
        {
            // Arrange
            var singleSubscriber = new List<SubscriberEntity>
            {
                new SubscriberEntity { SubscriberId = 100, SubscriberName = "SingleSubscriber" }
            };

            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns(singleSubscriber);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);

            var singleResult = result.First();
            singleResult.SubscriberId.Should().Be(100);
            singleResult.SubscriberName.Should().Be("SingleSubscriber");

            _mockDataAccess.Verify(x => x.GetSubscriberList(), Times.Once);
        }

        #endregion

        #region Constructor Tests

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SubscriberServices(null));
        }

        #endregion

        #region Edge Case Tests

        [Fact]
        public void GetSubscriberByName_WithWhitespaceString_ShouldCallDataAccess()
        {
            // Arrange
            var subscriberName = "   ";

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns((SubscriberEntity)null);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().BeNull();
            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        [Fact]
        public void GetSubscriberByName_WithVeryLongName_ShouldCallDataAccess()
        {
            // Arrange
            var subscriberName = new string('a', 1000); // Very long name
            var subscriberEntity = new SubscriberEntity
            {
                SubscriberId = 999,
                SubscriberName = subscriberName
            };

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns(subscriberEntity);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().NotBeNull();
            result.SubscriberId.Should().Be(999);
            result.SubscriberName.Should().Be(subscriberName);

            _mockDataAccess.Verify(x => x.GetSubscriberByName(subscriberName), Times.Once);
        }

        [Fact]
        public void GetSubscriberList_WithDuplicateIds_ShouldReturnAllItems()
        {
            // Arrange - Testing behavior with duplicate IDs (if allowed by business logic)
            var subscribersWithDuplicateIds = new List<SubscriberEntity>
            {
                new SubscriberEntity { SubscriberId = 1, SubscriberName = "Subscriber1" },
                new SubscriberEntity { SubscriberId = 1, SubscriberName = "Subscriber1Duplicate" },
                new SubscriberEntity { SubscriberId = 2, SubscriberName = "Subscriber2" }
            };

            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns(subscribersWithDuplicateIds);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            _mockDataAccess.Verify(x => x.GetSubscriberList(), Times.Once);
        }

        #endregion

        #region Data Transformation Tests

        [Fact]
        public void GetSubscriberByName_ShouldCorrectlyMapEntityToModel()
        {
            // Arrange
            var subscriberName = "MappingTestSubscriber";
            var subscriberEntity = new SubscriberEntity
            {
                SubscriberId = 42,
                SubscriberName = subscriberName
            };

            _mockDataAccess.Setup(x => x.GetSubscriberByName(subscriberName))
                          .Returns(subscriberEntity);

            // Act
            var result = _service.GetSubscriberByName(subscriberName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<SubscriberModel>();
            result.SubscriberId.Should().Be(subscriberEntity.SubscriberId);
            result.SubscriberName.Should().Be(subscriberEntity.SubscriberName);
        }

        [Fact]
        public void GetSubscriberList_ShouldCorrectlyMapEntitesToModels()
        {
            // Arrange
            var subscriberEntities = new List<SubscriberEntity>
            {
                new SubscriberEntity { SubscriberId = 10, SubscriberName = "TestSub1" },
                new SubscriberEntity { SubscriberId = 20, SubscriberName = "TestSub2" }
            };

            _mockDataAccess.Setup(x => x.GetSubscriberList())
                          .Returns(subscriberEntities);

            // Act
            var result = _service.GetSubscriberList();

            // Assert
            result.Should().NotBeNull();
            result.Should().AllBeOfType<SubscriberModel>();

            var resultList = result.ToList();
            resultList.Should().HaveCount(2);

            // Verify mapping of first item
            resultList[0].SubscriberId.Should().Be(10);
            resultList[0].SubscriberName.Should().Be("TestSub1");

            // Verify mapping of second item
            resultList[1].SubscriberId.Should().Be(20);
            resultList[1].SubscriberName.Should().Be("TestSub2");
        }

        #endregion
    }
}