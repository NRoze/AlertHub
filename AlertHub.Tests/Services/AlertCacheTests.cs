//using AlertHub.Api.Models;
//using AlertHub.Api.Options;
//using AlertHub.Api.Services;
//using Microsoft.Extensions.Options;
//using Moq;
//using StackExchange.Redis;

//namespace AlertHub.Tests.Services;

//public class AlertCacheTests
//{
//    private readonly Mock<IConnectionMultiplexer> _connectionMock;
//    private readonly Mock<IDatabase> _dbMock;
//    private readonly AlertCache _sut;

//    public AlertCacheTests()
//    {
//        var options = Options.Create(new RedisOptions());

//        _connectionMock = new Mock<IConnectionMultiplexer>();
//        _dbMock = new Mock<IDatabase>();

//        _connectionMock.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_dbMock.Object);

//        _sut = new AlertCache(_connectionMock.Object, options);
//    }

//[Fact]
//public async Task TryAddAsync_ReturnsTrue_WhenAlertIsNew()
//{
//    // Arrange
//    var alert = "1" ;
//    var cacheKey = "recent_alerts";
//    _dbMock.Setup(db => db.StringSetAsync(
//            It.Is<RedisKey>(k => k.ToString() == $"alert:{cacheKey}"),
//            It.Is<RedisValue>(v => v.ToString() == alert),
//            It.IsAny<TimeSpan>(),
//            When.NotExists))
//        .ReturnsAsync(true);

//    // Act
//    var result = await _sut.TryAddAsync(cacheKey);

//    // Assert
//    Assert.True(result);
//    _dbMock.Verify(db => db.StringSetAsync(
//            It.Is<RedisKey>(k => k.ToString() == $"alert:{cacheKey}"),
//            It.Is<RedisValue>(v => v.ToString() == alert),
//            It.IsAny<TimeSpan>(),
//            When.NotExists), Times.Once);
//}

//[Fact]
//public async Task TryAddAsync_ReturnsFalse_WhenAlertAlreadyExists()
//{
//    // Arrange
//    var alert = "1";
//    var cacheKey = "recent_alerts";
//    _dbMock.Setup(db => db.StringSetAsync(
//            It.Is<RedisKey>(k => k.ToString() == $"alert:{cacheKey}"),
//            It.Is<RedisValue>(v => v.ToString() == alert),
//            It.IsAny<TimeSpan>(),
//            When.NotExists))
//        .ReturnsAsync(false);

//    // Act
//    var result = await _sut.TryAddAsync(cacheKey);

//    // Assert
//    Assert.False(result);
//    _dbMock.Verify(db => db.StringSetAsync(
//            It.Is<RedisKey>(k => k.ToString() == $"alert:{cacheKey}"),
//            It.Is<RedisValue>(v => v.ToString() == alert),
//            It.IsAny<TimeSpan>(),
//            When.NotExists), Times.Once);
//}

//    //[Fact]
//    //public async Task TryAddRange_ShouldProcessAllItems()
//    //{
//    //    // Arrange
//    //    var alerts = new List<string> { "alert1", "alert2" };
//    //    var cacheKey = "recent_alerts";
//    //    _dbMock.Setup(db => db.SetAddAsync(cacheKey, It.IsAny<RedisValue>(), CommandFlags.None))
//    //        .ReturnsAsync(true);

//    //    // Act
//    //    await _sut.TryAddRange(alerts);

//    //    // Assert
//    //    _dbMock.Verify(db => db.SetAddAsync(cacheKey, "alert1", CommandFlags.None), Times.Once);
//    //    _dbMock.Verify(db => db.SetAddAsync(cacheKey, "alert2", CommandFlags.None), Times.Once);
//    //    _dbMock.Verify(db => db.KeyExpireAsync(cacheKey, TimeSpan.FromSeconds(30), ExpireWhen.Always, CommandFlags.None), Times.Exactly(2));
//    //}
//}
