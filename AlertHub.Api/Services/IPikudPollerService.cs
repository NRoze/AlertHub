using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace AlertHub.Api.Services
{
    public interface IPikudPollerService
    {
        Task<IReadOnlyList<string>> GetAlertsAsJson(CancellationToken cancellationToken);
    }
}
