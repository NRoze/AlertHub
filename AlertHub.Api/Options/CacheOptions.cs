namespace AlertHub.Api.Options;

public class CacheOptions
{
    public TimeSpan AlertExpiry { get; set; } = TimeSpan.FromHours(24);
}
