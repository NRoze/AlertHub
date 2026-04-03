using StackExchange.Redis;

namespace AlertHub.Api.Infrastructure;

public static class RedisConnection
{
    public static readonly RedisChannel AlertsChannel = RedisChannel.Literal("alerts_channel");
}