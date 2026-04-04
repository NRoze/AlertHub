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
        var random = Random.Shared.Next(1, 5);

        return [await File.ReadAllTextAsync($"Samples/FinishedSample{random}.json", Encoding.UTF8, cancellationToken)];
    }
}
