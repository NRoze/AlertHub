using StackExchange.Redis;

namespace AlertHub.Api.Options;

public class RedisOptions
{
    public RedisChannel AlertsChannel { get; set; } = RedisChannel.Literal("alerts_channel");
}
