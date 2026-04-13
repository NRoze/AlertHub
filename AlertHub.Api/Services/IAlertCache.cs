using AlertHub.Api.Models;
using System.Collections.Immutable;

namespace AlertHub.Api.Services;

public interface IAlertCache
{
    bool TryAdd(AlertMessageDto alert);

    //void TryAddRange(ImmutableArray<AlertLocationDto> alerts);

    ImmutableArray<AlertMessageDto> GetAll();
}
