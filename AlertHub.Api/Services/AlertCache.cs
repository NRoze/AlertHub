using AlertHub.Api.Infrastructure;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace AlertHub.Api.Services;

public class AlertCache : IAlertCache
{
    private readonly IDatabase _db;
    private const string CacheKey = "recent_alerts";

    public AlertCache(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }

    public async Task<bool> TryAddAsync(string alert)
{
        var added = await _db.SetAddAsync(CacheKey, alert);

        if (added)
        {
            await _db.KeyExpireAsync(CacheKey, TimeSpan.FromSeconds(30));
        }

        return added;
    }

    public async Task TryAddRange(IReadOnlyList<string> alerts)
    {
        foreach (var alert in alerts)
            await TryAddAsync(alert);
    }
}