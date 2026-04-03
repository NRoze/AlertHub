using StackExchange.Redis;

namespace AlertHub.Api.Options;

internal sealed class RedisOptions
{
    public RedisChannel AlertsChannel { get; set; } = RedisChannel.Literal("alerts_channel");
}
