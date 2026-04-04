using Microsoft.Extensions.Hosting;
using AlertHub.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlertHub.Api.Options;
using AlertHub.Api.Middleware;
using StackExchange.Redis;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Threading;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerOptions =>
    {
        workerOptions.UseMiddleware<GlobalExceptionMiddleware>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(context.HostingEnvironment.ContentRootPath);
        config.AddJsonFile("appsettings.function.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IConnectionMultiplexer>(sp => 
        {
            var connString = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ??
                context.Configuration["Redis:ConnectionString"]!;

            var options = ConfigurationOptions.Parse(connString);
            options.AbortOnConnectFail = false;
            
            // Helpful for Azure issues: ensure thread pool has enough minimum capacity to burst
            ThreadPool.SetMinThreads(200, 200);

            return ConnectionMultiplexer.Connect(options);
        });

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