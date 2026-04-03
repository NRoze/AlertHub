using AlertHub.Api.Models;

namespace AlertHub.Api.Services;

internal sealed class AlertService : IAlertService
{
    public async Task<bool> ProcessAlertAsync(AlertMessage alert)
    {
        // TODO: integrate idempotency check here
        // return false if duplicate

        return true;
    }
}
