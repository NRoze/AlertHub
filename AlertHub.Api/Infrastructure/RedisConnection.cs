using StackExchange.Redis;

namespace AlertHub.Api.Infrastructure;

public static class RedisConnection
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new(() =>
        ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION")!)
    );

    public static ConnectionMultiplexer Connection => LazyConnection.Value;

    public static readonly RedisChannel AlertsChannel = RedisChannel.Literal("alerts_channel");
}