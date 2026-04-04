using AlertHub.Api.Models;

namespace AlertHub.Api.Services;

public interface IAlertCache
{
    Task<bool> TryAddAsync(string alertId, CancellationToken cancellationToken = default);
    //Task TryAddRange(IReadOnlyList<string> alerts, CancellationToken cancellationToken = default);
}
