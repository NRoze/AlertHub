using AlertHub.Api.Models;
using System.Collections.Immutable;

namespace AlertHub.Api.Services;

public interface IAlertCache
{
    bool TryAdd(AlertLocationDto alert);

    void TryAddRange(ImmutableArray<AlertLocationDto> alerts);

    ImmutableArray<AlertLocationDto> GetAll();
}
