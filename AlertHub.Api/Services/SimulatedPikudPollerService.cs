using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace AlertHub.Api.Services;

internal class SimulatedPikudPollerService : IPikudPollerService
{
    private static ImmutableList<string> _samples = [];
    private static readonly Lock _lock = new();

    private readonly Stopwatch _stopwatch = new();
    private bool _initialized = false;

    private async Task Initialize(CancellationToken cancellationToken)
    { 
        if (_initialized) return;

        lock (_lock)
        {
            if (_initialized) return;

            var tempSamples = new List<string>();
            for (int i = 1; i <= 3; i++)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Samples/FinishedSample{i}.json");
                tempSamples.Add(File.ReadAllText(path, Encoding.UTF8));
            }

            _samples = [.. tempSamples];
            _stopwatch.Restart();
            _initialized = true;
        }
    }
    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken)
    {
        await Initialize(cancellationToken);

        string selectedSample = _stopwatch.Elapsed switch
        {
            { TotalSeconds: < 10 } => _samples[0],
            { TotalSeconds: < 20 } => _samples[1],
            { TotalSeconds: < 30 } => _samples[2],
            _ => _samples[2]
        };

        if (string.IsNullOrWhiteSpace(selectedSample))
        {
            //_stopwatch.Restart();
            return [];
        }

        return [selectedSample];
    }
}
