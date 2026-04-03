using AlertHub.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AlertHub.Api.Services;

internal class SimulatedPikudPollerService : IPikudPollerService
{
    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync("Samples/FinishedSample1.json", cancellationToken);

        return [json];
    }
}
