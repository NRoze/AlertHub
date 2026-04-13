using AlertHub.Api.Models;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace AlertHub.Tests.Services;

public class AlertCacheTests
{
    private readonly MemoryCache _memoryCache;
    private readonly CacheOptions _options;
    private readonly AlertCache _sut;

    public AlertCacheTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _options = new CacheOptions { AlertExpiry = TimeSpan.FromMinutes(5) };

        _sut = new AlertCache(_memoryCache, Options.Create(_options), NullLogger<AlertCache>.Instance);
    }

    [Fact]
    public void TryAdd_ReturnsTrue_WhenAlertIsNew()
    {
        var alert = new AlertMessageDto { Id = "alert-1", Timestamp = 1000 };
        var result = _sut.TryAdd(alert);

        Assert.True(result);
        Assert.Equal(1000, alert.Timestamp);
        Assert.True(_memoryCache.TryGetValue("alert-1", out _));
    }

    [Fact]
    public void TryAdd_ReturnsFalse_WhenAlertAlreadyExists()
    {
        var alert = new AlertMessageDto { Id = "alert-1", Timestamp = 1000 };
        _sut.TryAdd(alert);

        var result = _sut.TryAdd(alert);

        Assert.False(result);
    }

    [Fact]
    public void GetAll_ReturnsActiveAlerts()
    {
        var alert1 = new AlertMessageDto { Id = "alert-1" };
        var alert2 = new AlertMessageDto { Id = "alert-2" };

        _sut.TryAdd(alert1);
        _sut.TryAdd(alert2);

        var results = _sut.GetAll();

        Assert.Equal(2, results.Length);
        Assert.Contains(results, a => a.Id == "alert-1");
        Assert.Contains(results, a => a.Id == "alert-2");
    }
}
