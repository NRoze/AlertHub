using StackExchange.Redis;

namespace AlertHub.Api.Options;

public sealed class RedisOptions
{
    public TimeSpan AlertExpiry { get; set; } = TimeSpan.FromSeconds(10);
    public RedisChannel AlertsChannel { get; set; } = RedisChannel.Literal("alerts_channel");
}
