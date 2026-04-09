using AlertHub.Api.Middleware;
using AlertHub.Api.Options;
using AlertHub.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerOptions =>
    {
        workerOptions.UseMiddleware<GlobalExceptionMiddleware>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(context.HostingEnvironment.ContentRootPath);
        config.AddJsonFile("appsettings.function.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddMemoryCache();
        services.AddHttpClient("PikudClient")
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            UseCookies = true,
            CookieContainer = new CookieContainer(),
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        }); ;
        services.Configure<PikudPollerOptions>(
            context.Configuration.GetSection("PikudPoller"));
        
        var useSimulatedAlerts = context.Configuration.GetValue<bool>("PikudPoller:UseSimulatedAlerts");

        if (useSimulatedAlerts)
        {
            services.AddSingleton<IPikudPollerService, SimulatedPikudPollerService>();
        }
        else
        {
            services.AddSingleton<IPikudPollerService, PikudPollerService>();
        }

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IAlertCache, AlertCache>();
    })
    .Build();

host.Run();