using Device.Fingerprint.Redis;
using FluentAssertions;
using Moq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Device.Fingerprint.Test.Redis
{
    /// <summary>
    /// Unit tests for RedisMultiplexService class.
    /// Tests Redis operations with mocked IConnectionMultiplexer and IDatabase.
    /// </summary>
    public class RedisMultiplexServiceTests
    {
        private const int EverTaggingDbIndex = 5;

        private Mock<IConnectionMultiplexer> CreateMockConnectionMultiplexer(out Mock<IDatabase> mockEverTaggingDb, out Mock<IDatabase> mockDatabase)
        {
            var mockConnectionMultiplexer = new Mock<IConnectionMultiplexer>();
            mockEverTaggingDb = new Mock<IDatabase>();
            mockDatabase = new Mock<IDatabase>();

            // Setup GetDatabase to return the correct mock based on index
            mockConnectionMultiplexer.Setup(m => m.GetDatabase(EverTaggingDbIndex, null))
                .Returns(mockEverTaggingDb.Object);
            mockConnectionMultiplexer.Setup(m => m.GetDatabase(It.Is<int>(i => i != EverTaggingDbIndex), null))
                .Returns(mockDatabase.Object);

            return mockConnectionMultiplexer;
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidParameters_ShouldInitialize()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);

            // Act
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);

            // Assert
            service.Should().NotBeNull();
            service.Should().BeAssignableTo<IRedisMultiplexService>();
            mockConnectionMultiplexer.Verify(m => m.GetDatabase(EverTaggingDbIndex, null), Times.Once);
        }

        [Fact]
        public void Constructor_ShouldInitializeEverTaggingDb()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);

            // Act
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var db = service.GetRedisEverTaggingDb();

            // Assert
            db.Should().NotBeNull();
        }

        #endregion

        #region GetRedisEverTaggingDb Tests

        [Fact]
        public void GetRedisEverTaggingDb_ShouldReturnCorrectDatabase()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);

            // Act
            var result = service.GetRedisEverTaggingDb();

            // Assert
            result.Should().NotBeNull();
        }

        #endregion

        #region HashSetMultipleFieldsWithDbIndexAsync Tests

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_WithValidData_ShouldSetAllFields()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKeyValuePairs = new Dictionary<string, string>
            {
                { "field1", "value1" },
                { "field2", "value2" },
                { "field3", "value3" }
            };
            var expireSecond = 3600L;
            var dbIndex = 0;

            mockDatabase.Setup(m => m.HashSetAsync(hashKey, It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), When.Always, CommandFlags.None))
                .ReturnsAsync(true);
            mockDatabase.Setup(m => m.KeyExpireAsync(hashKey, It.IsAny<TimeSpan?>(), ExpireWhen.Always, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await service.HashSetMultipleFieldsWithDbIndexAsync(dbIndex, hashKey, fieldKeyValuePairs, expireSecond);

            // Assert
            mockDatabase.Verify(m => m.HashSetAsync(hashKey, "field1", "value1", When.Always, CommandFlags.None), Times.Once);
            mockDatabase.Verify(m => m.HashSetAsync(hashKey, "field2", "value2", When.Always, CommandFlags.None), Times.Once);
            mockDatabase.Verify(m => m.HashSetAsync(hashKey, "field3", "value3", When.Always, CommandFlags.None), Times.Once);
            mockDatabase.Verify(m => m.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(expireSecond), ExpireWhen.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_WithEmptyDictionary_ShouldOnlySetExpiry()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKeyValuePairs = new Dictionary<string, string>();
            var expireSecond = 1800L;
            var dbIndex = 1;

            mockDatabase.Setup(m => m.KeyExpireAsync(hashKey, It.IsAny<TimeSpan?>(), ExpireWhen.Always, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await service.HashSetMultipleFieldsWithDbIndexAsync(dbIndex, hashKey, fieldKeyValuePairs, expireSecond);

            // Assert
            mockDatabase.Verify(m => m.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Never);
            mockDatabase.Verify(m => m.KeyExpireAsync(hashKey, TimeSpan.FromSeconds(expireSecond), ExpireWhen.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task HashSetMultipleFieldsWithDbIndexAsync_WithDifferentDbIndex_ShouldUseCorrectDatabase()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKeyValuePairs = new Dictionary<string, string> { { "key", "value" } };
            var expireSecond = 600L;
            var dbIndex = 3;

            var specificMockDb = new Mock<IDatabase>();
            mockConnectionMultiplexer.Setup(m => m.GetDatabase(dbIndex, null))
                .Returns(specificMockDb.Object);
            specificMockDb.Setup(m => m.HashSetAsync(hashKey, It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), When.Always, CommandFlags.None))
                .ReturnsAsync(true);
            specificMockDb.Setup(m => m.KeyExpireAsync(hashKey, It.IsAny<TimeSpan?>(), ExpireWhen.Always, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await service.HashSetMultipleFieldsWithDbIndexAsync(dbIndex, hashKey, fieldKeyValuePairs, expireSecond);

            // Assert
            mockConnectionMultiplexer.Verify(m => m.GetDatabase(dbIndex, null), Times.Once);
            specificMockDb.Verify(m => m.HashSetAsync(hashKey, "key", "value", When.Always, CommandFlags.None), Times.Once);
        }

        #endregion

        #region HashGetWithDbIndexAsync Tests

        [Fact]
        public async Task HashGetWithDbIndexAsync_WithExistingField_ShouldReturnValue()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKey = "field1";
            var expectedValue = "value1";
            var dbIndex = 0;

            mockDatabase.Setup(m => m.HashGetAsync(hashKey, fieldKey, CommandFlags.None))
                .ReturnsAsync((RedisValue)expectedValue);

            // Act
            var result = await service.HashGetWithDbIndexAsync(dbIndex, hashKey, fieldKey);

            // Assert
            result.Should().Be(expectedValue);
            mockDatabase.Verify(m => m.HashGetAsync(hashKey, fieldKey, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task HashGetWithDbIndexAsync_WithNonExistingField_ShouldReturnNull()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKey = "nonexistent";
            var dbIndex = 1;

            mockDatabase.Setup(m => m.HashGetAsync(hashKey, fieldKey, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await service.HashGetWithDbIndexAsync(dbIndex, hashKey, fieldKey);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task HashGetWithDbIndexAsync_WithDifferentDbIndex_ShouldUseCorrectDatabase()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var fieldKey = "field1";
            var dbIndex = 2;
            var specificMockDb = new Mock<IDatabase>();

            mockConnectionMultiplexer.Setup(m => m.GetDatabase(dbIndex, null))
                .Returns(specificMockDb.Object);
            specificMockDb.Setup(m => m.HashGetAsync(hashKey, fieldKey, CommandFlags.None))
                .ReturnsAsync((RedisValue)"value");

            // Act
            await service.HashGetWithDbIndexAsync(dbIndex, hashKey, fieldKey);

            // Assert
            mockConnectionMultiplexer.Verify(m => m.GetDatabase(dbIndex, null), Times.Once);
            specificMockDb.Verify(m => m.HashGetAsync(hashKey, fieldKey, CommandFlags.None), Times.Once);
        }

        #endregion

        #region GetTimeSpan Tests

        [Fact]
        public async Task GetTimeSpan_WithExistingKey_ShouldReturnTotalSeconds()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var dbIndex = 0;
            var ttl = TimeSpan.FromSeconds(3600);

            mockDatabase.Setup(m => m.KeyTimeToLiveAsync(hashKey, CommandFlags.None))
                .ReturnsAsync(ttl);

            // Act
            var result = await service.GetTimeSpan(dbIndex, hashKey);

            // Assert
            result.Should().Be(3600.0);
            mockDatabase.Verify(m => m.KeyTimeToLiveAsync(hashKey, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task GetTimeSpan_WithNonExistingKey_ShouldReturnMinusOne()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "nonexistent:hash";
            var dbIndex = 0;

            mockDatabase.Setup(m => m.KeyTimeToLiveAsync(hashKey, CommandFlags.None))
                .ReturnsAsync((TimeSpan?)null);

            // Act
            var result = await service.GetTimeSpan(dbIndex, hashKey);

            // Assert
            result.Should().Be(-1);
        }

        [Fact]
        public async Task GetTimeSpan_WithKeyWithoutExpiry_ShouldReturnMinusOne()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "persistent:hash";
            var dbIndex = 1;

            mockDatabase.Setup(m => m.KeyTimeToLiveAsync(hashKey, CommandFlags.None))
                .ReturnsAsync((TimeSpan?)null);

            // Act
            var result = await service.GetTimeSpan(dbIndex, hashKey);

            // Assert
            result.Should().Be(-1);
        }

        #endregion

        #region EverTagging_AddItemWithExpiryAsync Tests

        [Fact]
        public async Task EverTagging_AddItemWithExpiryAsync_WithValidData_ShouldSetValueAndExpiry()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var itemKey = "item1";
            var value = "testValue";
            var expiry = TimeSpan.FromMinutes(30);
            var expectedKey = $"{hashKey}:{itemKey}";

            mockEverTaggingDb.Setup(m => m.StringSetAsync(expectedKey, value, null, false, When.Always, CommandFlags.None))
                .ReturnsAsync(true);
            mockEverTaggingDb.Setup(m => m.KeyExpireAsync(expectedKey, expiry, ExpireWhen.Always, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await service.EverTagging_AddItemWithExpiryAsync(hashKey, itemKey, value, expiry);

            // Assert
            mockEverTaggingDb.Verify(m => m.StringSetAsync(expectedKey, value, null, false, When.Always, CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.KeyExpireAsync(expectedKey, expiry, ExpireWhen.Always, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task EverTagging_AddItemWithExpiryAsync_WithEmptyValue_ShouldStillSetKey()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var hashKey = "test:hash";
            var itemKey = "item1";
            var value = string.Empty;
            var expiry = TimeSpan.FromSeconds(60);
            var expectedKey = $"{hashKey}:{itemKey}";

            mockEverTaggingDb.Setup(m => m.StringSetAsync(expectedKey, value, null, false, When.Always, CommandFlags.None))
                .ReturnsAsync(true);
            mockEverTaggingDb.Setup(m => m.KeyExpireAsync(expectedKey, expiry, ExpireWhen.Always, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            await service.EverTagging_AddItemWithExpiryAsync(hashKey, itemKey, value, expiry);

            // Assert
            mockEverTaggingDb.Verify(m => m.StringSetAsync(expectedKey, value, null, false, When.Always, CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.KeyExpireAsync(expectedKey, expiry, ExpireWhen.Always, CommandFlags.None), Times.Once);
        }

        #endregion

        #region EverTagging_GetValueFromRedisAsync Tests

        [Fact]
        public async Task EverTagging_GetValueFromRedisAsync_WithExistingValue_ShouldReturnValue()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var key = "test:key";
            var expectedValue = "testValue";

            mockEverTaggingDb.Setup(m => m.StringGetAsync(key, CommandFlags.None))
                .ReturnsAsync((RedisValue)expectedValue);

            // Act
            var result = await service.EverTagging_GetValueFromRedisAsync(key);

            // Assert
            result.Should().Be(expectedValue);
            mockEverTaggingDb.Verify(m => m.StringGetAsync(key, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task EverTagging_GetValueFromRedisAsync_WithNonExistingValue_ShouldReturnEmptyString()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var key = "nonexistent:key";

            mockEverTaggingDb.Setup(m => m.StringGetAsync(key, CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await service.EverTagging_GetValueFromRedisAsync(key);

            // Assert
            result.Should().Be(string.Empty);
        }

        #endregion

        #region EverTagging_GetValuesFromRedis Tests

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithFirstValueExists_ShouldReturnFirstKey()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var keys = new List<string> { "key1", "key2", "key3" };

            mockEverTaggingDb.Setup(m => m.StringGetAsync("key1", CommandFlags.None))
                .ReturnsAsync((RedisValue)"value1");

            // Act
            var result = await service.EverTagging_GetValuesFromRedis(keys);

            // Assert
            result.Should().Be("key1");
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key1", CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key2", CommandFlags.None), Times.Never);
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithSecondValueExists_ShouldReturnSecondKey()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var keys = new List<string> { "key1", "key2", "key3" };

            mockEverTaggingDb.Setup(m => m.StringGetAsync("key1", CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);
            mockEverTaggingDb.Setup(m => m.StringGetAsync("key2", CommandFlags.None))
                .ReturnsAsync((RedisValue)"value2");

            // Act
            var result = await service.EverTagging_GetValuesFromRedis(keys);

            // Assert
            result.Should().Be("key2");
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key1", CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key2", CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key3", CommandFlags.None), Times.Never);
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithNoValueExists_ShouldReturnEmptyString()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var keys = new List<string> { "key1", "key2", "key3" };

            mockEverTaggingDb.Setup(m => m.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await service.EverTagging_GetValuesFromRedis(keys);

            // Assert
            result.Should().Be(string.Empty);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key1", CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key2", CommandFlags.None), Times.Once);
            mockEverTaggingDb.Verify(m => m.StringGetAsync("key3", CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task EverTagging_GetValuesFromRedis_WithEmptyList_ShouldReturnEmptyString()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var keys = new List<string>();

            // Act
            var result = await service.EverTagging_GetValuesFromRedis(keys);

            // Assert
            result.Should().Be(string.Empty);
            mockEverTaggingDb.Verify(m => m.StringGetAsync(It.IsAny<RedisKey>(), CommandFlags.None), Times.Never);
        }

        #endregion

        #region EverTagging_DeleteValueFromRedis Tests

        [Fact]
        public async Task EverTagging_DeleteValueFromRedis_WithExistingKey_ShouldReturnTrue()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var key = "test:key";

            mockEverTaggingDb.Setup(m => m.KeyDeleteAsync(key, CommandFlags.None))
                .ReturnsAsync(true);

            // Act
            var result = await service.EverTagging_DeleteValueFromRedis(key);

            // Assert
            result.Should().BeTrue();
            mockEverTaggingDb.Verify(m => m.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task EverTagging_DeleteValueFromRedis_WithNonExistingKey_ShouldReturnFalse()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);
            var key = "nonexistent:key";

            mockEverTaggingDb.Setup(m => m.KeyDeleteAsync(key, CommandFlags.None))
                .ReturnsAsync(false);

            // Act
            var result = await service.EverTagging_DeleteValueFromRedis(key);

            // Assert
            result.Should().BeFalse();
            mockEverTaggingDb.Verify(m => m.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
        }

        #endregion

        #region Interface Tests

        [Fact]
        public void IRedisMultiplexService_ShouldHaveHashSetMultipleFieldsWithDbIndexAsyncMethod()
        {
            // Arrange
            var method = typeof(IRedisMultiplexService).GetMethod("HashSetMultipleFieldsWithDbIndexAsync");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task));
            method.GetParameters().Should().HaveCount(4);
        }

        [Fact]
        public void IRedisMultiplexService_ShouldHaveHashGetWithDbIndexAsyncMethod()
        {
            // Arrange
            var method = typeof(IRedisMultiplexService).GetMethod("HashGetWithDbIndexAsync");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task<string>));
            method.GetParameters().Should().HaveCount(3);
        }

        [Fact]
        public void IRedisMultiplexService_ShouldHaveGetTimeSpanMethod()
        {
            // Arrange
            var method = typeof(IRedisMultiplexService).GetMethod("GetTimeSpan");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(Task<double>));
            method.GetParameters().Should().HaveCount(2);
        }

        [Fact]
        public void IRedisMultiplexService_ShouldHaveGetRedisEverTaggingDbMethod()
        {
            // Arrange
            var method = typeof(IRedisMultiplexService).GetMethod("GetRedisEverTaggingDb");

            // Assert
            method.Should().NotBeNull();
            method!.ReturnType.Should().Be(typeof(IDatabase));
            method.GetParameters().Should().HaveCount(0);
        }

        [Fact]
        public void IRedisMultiplexService_ShouldHaveAllEverTaggingMethods()
        {
            // Arrange
            var interfaceType = typeof(IRedisMultiplexService);

            // Assert
            interfaceType.GetMethod("EverTagging_AddItemWithExpiryAsync").Should().NotBeNull();
            interfaceType.GetMethod("EverTagging_GetValueFromRedisAsync").Should().NotBeNull();
            interfaceType.GetMethod("EverTagging_GetValuesFromRedis").Should().NotBeNull();
            interfaceType.GetMethod("EverTagging_DeleteValueFromRedis").Should().NotBeNull();
        }

        [Fact]
        public void RedisMultiplexService_ShouldImplementIRedisMultiplexService()
        {
            // Arrange
            var mockConnectionMultiplexer = CreateMockConnectionMultiplexer(out var mockEverTaggingDb, out var mockDatabase);
            var service = new RedisMultiplexService(mockConnectionMultiplexer.Object, EverTaggingDbIndex);

            // Assert
            service.Should().BeAssignableTo<IRedisMultiplexService>();
        }

        #endregion
    }
}
