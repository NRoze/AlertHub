using AlertHub.Api.Models;
using AlertHub.Api.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AlertHub.Api.Services;

internal sealed class AlertCache : IAlertCache
{
    private readonly IDatabase _db;
    private readonly RedisOptions _options;

    private static readonly RedisValue AlertValue= new("1");
    private static RedisKey AlertKey(string alertId) => new($"alert:{alertId}");

    public AlertCache(IConnectionMultiplexer multiplexer, IOptions<RedisOptions> options)
    {
        _db = multiplexer.GetDatabase();
        _options = options.Value;
    }
    public async Task<bool> TryAddAsync(string alertId, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();

        return await _db.StringSetAsync(
            AlertKey(alertId),
            AlertValue,
            expiry: _options.AlertExpiry,
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