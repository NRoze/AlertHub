namespace AlertHub.Api.Services;

public interface IAlertCache
{
    Task<bool> TryAddAsync(string alert, CancellationToken cancellationToken = default);
    Task TryAddRange(IReadOnlyList<string> alerts, CancellationToken cancellationToken = default);
}
