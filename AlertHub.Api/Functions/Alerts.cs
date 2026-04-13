using AlertHub.Api.Models;
using AlertHub.Api.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using System.Collections.Immutable;

namespace AlertHub.Api.Functions;

internal sealed class AlertsFunction
{
    private readonly IAlertCache _cache;

    public AlertsFunction(IAlertCache alertCache)
    {
        _cache = alertCache;
    }

    [Function("alerts")]
    public async Task<ImmutableArray<AlertMessageDto>> Alerts(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
    {
        return _cache.GetAll(); ;
    }
}
