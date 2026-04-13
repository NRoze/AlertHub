using AlertHub.Api.Models;
using AlertHub.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace AlertHub.Api.Services;

internal sealed class AlertCache : IAlertCache
{
    private readonly IMemoryCache _cache;
    private readonly CacheOptions _options;
    private readonly ILogger<AlertCache> _logger;
    private readonly ConcurrentDictionary<string, byte> _activeKeys = new();
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    public AlertCache(IMemoryCache cache, IOptions<CacheOptions> options, ILogger<AlertCache> logger)
    {
        _cache = cache;
        _options = options.Value;
        _logger = logger;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(_options.AlertExpiry)
            .RegisterPostEvictionCallback(OnEvicted);
    }

    public bool TryAdd(AlertMessageDto alert)
    {
        if (_cache.TryGetValue(alert.Id, out _)) return false;

        _cache.Set(alert.Id, alert, _cacheEntryOptions);
        _activeKeys.TryAdd(alert.Id, 0);

        return true;
    }

    public ImmutableArray<AlertMessageDto> GetAll()
    {
        var alerts = new List<AlertMessageDto>();

        foreach (var key in _activeKeys.Keys)
        {
            if (_cache.TryGetValue(key, out AlertMessageDto? alert) && alert is not null)
            {
                alerts.Add(alert);
            }
        }

        return [.. alerts];
    }

    private void OnEvicted(object key, object? value, EvictionReason reason, object? state)
    {
        if (reason is EvictionReason.Expired or EvictionReason.Removed or EvictionReason.Capacity)
        {
            if (key is string id)
            {
                _activeKeys.TryRemove(id, out _);
            }
        }
    }

    //public void TryAddRange(ImmutableArray<AlertMessageDto> alerts)
    //{
    //    foreach (var alert in alerts)
    //    {
    //        if (alert is not null) TryAdd(alert);
    //    }
    //}
}