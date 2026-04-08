using AlertHub.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AlertHub.Api.Services;

internal class SimulatedPikudPollerService : IPikudPollerService
{
    private int _iteration = 0;
    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        Interlocked.Increment(ref _iteration);
        return [await File.ReadAllTextAsync(
            $"Samples/FinishedSample{(_iteration%3)+1}.json", Encoding.UTF8, cancellationToken)];
    }
}
