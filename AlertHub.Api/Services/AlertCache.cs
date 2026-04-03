using AlertHub.Api.Models;
using StackExchange.Redis;

namespace AlertHub.Api.Services;

internal sealed class AlertCache : IAlertCache
{
    private readonly IDatabase _db;
    private const string CacheKey = "recent_alerts";
    public static readonly TimeSpan Expiry = TimeSpan.FromSeconds(30);
    private static RedisValue AlertValue(string alert) => new(alert);
    private static RedisKey AlertKey(string alertId) => new($"alert:{alertId}");
    public AlertCache(IConnectionMultiplexer multiplexer)
    {
        _db = multiplexer.GetDatabase();
    }
    public async Task<bool> TryAddAsync(string alertId, string alertRaw, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return await _db.StringSetAsync(
            AlertKey(alertId),
            AlertValue(alertRaw),
            expiry: Expiry,
            when: When.NotExists
        );
    }

//    public async Task<bool> TryAddAsync(string alert, CancellationToken cancellationToken = default)
//{
//        cancellationToken.ThrowIfCancellationRequested();

//        var added = await _db.SetAddAsync(CacheKey, alert);

//        if (added)
//        {
//            await _db.KeyExpireAsync(CacheKey, TimeSpan.FromSeconds(30));
//        }

//        return added;
//    }

    //public async Task TryAddRange(IReadOnlyList<string> alerts, CancellationToken cancellationToken = default)
    //{
    //    cancellationToken.ThrowIfCancellationRequested();

    //    foreach (var alert in alerts)
    //        await TryAddAsync(alert, cancellationToken);
    //}
}