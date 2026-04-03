using Microsoft.Extensions.Hosting;
using AlertHub.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlertHub.Api.Options;
using AlertHub.Api.Middleware;
using StackExchange.Redis;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerOptions =>
    {
        workerOptions.UseMiddleware<GlobalExceptionMiddleware>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(context.HostingEnvironment.ContentRootPath);
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var r = context.Configuration["Redis:ConnectionString"];
        
        services.AddSingleton<IConnectionMultiplexer>(sp => 
            ConnectionMultiplexer.Connect(
                Environment.GetEnvironmentVariable("REDIS_CONNECTION") ??
                context.Configuration["Redis:ConnectionString"]!));

        services.AddHttpClient();
        services.Configure<PikudPollerOptions>(
            context.Configuration.GetSection("PikudPoller"));
        
        var useSimulatedAlerts = context.Configuration.GetSection("PikudPoller")
                                    .Get<PikudPollerOptions>()?.UseSimulatedAlerts == true;
        if (useSimulatedAlerts)
        {
            services.AddScoped<IPikudPollerService, SimulatedPikudPollerService>();
        }
        else
        {
            services.AddScoped<IPikudPollerService, PikudPollerService>();
        }

        services.AddScoped<IAlertCache, AlertCache>();
    })
    .Build();


host.Run();