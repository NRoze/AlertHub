using AlertHub.Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace AlertHub.Api.Services;

public interface IAlertService
{
    Task<bool> ProcessAlertAsync(AlertMessage alert);
}
