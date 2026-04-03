namespace AlertHub.Api.Services;

public interface IAlertCache
{
    Task<bool> TryAddAsync(string alert);
    Task TryAddRange(IReadOnlyList<string> alerts);
}
