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
        var json1 = await File.ReadAllTextAsync("Samples/FinishedSample1.json", Encoding.UTF8, cancellationToken);
        var json2 = await File.ReadAllTextAsync("Samples/FinishedSample2.json", Encoding.UTF8, cancellationToken);
        var json3 = await File.ReadAllTextAsync("Samples/FinishedSample3.json", Encoding.UTF8, cancellationToken);

        return [/*json1, json2,*/ json3];
    }
}
