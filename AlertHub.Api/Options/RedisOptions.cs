using StackExchange.Redis;

namespace AlertHub.Api.Options;

public sealed class RedisOptions
{
    public TimeSpan AlertExpiry = TimeSpan.FromSeconds(6);
    public RedisChannel AlertsChannel { get; set; } = RedisChannel.Literal("alerts_channel");
}
