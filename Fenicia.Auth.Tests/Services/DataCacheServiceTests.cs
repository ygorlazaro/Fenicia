namespace Fenicia.Auth.Tests.Services
{
    using System.Text.Json;
    using System.Threading.Tasks;
    using Fenicia.Auth.Domains.DataCache;
    using Microsoft.Extensions.Logging;
    using Moq;
    using NUnit.Framework;
    using StackExchange.Redis;

    [TestFixture]
    public class DataCacheServiceTests
    {
        private RedisDataCacheService service;
        private Mock<IDatabase> dbMock;
        private Mock<ILogger<RedisDataCacheService>> loggerMock;
        private Mock<IConnectionMultiplexer> redisMock;

        [SetUp]
        public void SetUp()
        {
            dbMock = new Mock<IDatabase>();
            loggerMock = new Mock<ILogger<RedisDataCacheService>>();
            redisMock = new Mock<IConnectionMultiplexer>();
            redisMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(dbMock.Object);
            service = new RedisDataCacheService(redisMock.Object, loggerMock.Object);
        }

        [Test]
        public async Task GetAsyncReturnsDeserializedObjectOnCacheHit()
        {
            var key = "test-key";
            var expected = new TestData { Value = "abc" };
            var json = JsonSerializer.Serialize(expected);
            dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(json);

            var result = await service.GetAsync<TestData>(key);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expected.Value));
        }

        [Test]
        public async Task GetAsyncReturnsDefaultOnCacheMiss()
        {
            var key = "missing-key";
            dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

            var result = await service.GetAsync<TestData>(key);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetAsyncThrowsOnRedisConnectionException()
        {
            var key = "err-key";
            dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.SocketFailure, "fail"));
            try
            {
                await service.GetAsync<TestData>(key);
                Assert.Fail("Expected RedisConnectionException was not thrown.");
            }
            catch (RedisConnectionException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public async Task GetAsyncThrowsOnJsonException()
        {
            var key = "bad-json";
            dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync("not-json");
            try
            {
                await service.GetAsync<TestData>(key);
                Assert.Fail("Expected JsonException was not thrown.");
            }
            catch (JsonException)
            {
                Assert.Pass();
            }
        }

        [Test]
        public async Task RemoveAsyncRemovesKey()
        {
            var key = "remove-key";
            dbMock.Setup(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(true);
            await service.RemoveAsync(key);
            dbMock.Verify(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
        }

        [Test]
        public async Task RemoveAsyncThrowsOnRedisConnectionException()
        {
            var key = "err-remove";
            dbMock.Setup(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.SocketFailure, "fail"));
            try
            {
                await service.RemoveAsync(key);
                Assert.Fail("Expected RedisConnectionException was not thrown.");
            }
            catch (RedisConnectionException)
            {
                Assert.Pass();
            }
        }

        public class TestData
        {
            public string? Value { get; set; }
        }
    }
}
