using AlertHub.Api.Services;
using System.Diagnostics;
using System.Text;

internal class SimulatedPikudPollerService : IPikudPollerService
{
    private readonly string[] _samples = new string[4];
    private bool _isLoaded = false;
    private static readonly Stopwatch _globalTimer = Stopwatch.StartNew();

    public async Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken ct)
    {
        if (!_isLoaded) LoadFiles();

        double elapsed = _globalTimer.Elapsed.TotalSeconds % 60; 
        string selected = elapsed switch
        {
            < 10 => _samples[0],
            < 20 => _samples[1],
            < 30 => _samples[2],
            < 40 => _samples[3],
            _ => string.Empty 
        };

        return string.IsNullOrEmpty(selected) ? [] : [selected];
    }

    private void LoadFiles()
    {
        for (int i = 1; i <= 4; i++)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Samples/FinishedSample{i}.json");
            _samples[i - 1] = File.ReadAllText(path, Encoding.UTF8);
        }
        _isLoaded = true;
    }
}