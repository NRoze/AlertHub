using Microsoft.Extensions.Hosting;
using AlertHub.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlertHub.Api.Options;
using AlertHub.Api.Middleware;
using StackExchange.Redis;

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
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var connString = context.Configuration["Redis:ConnectionString"];
            var options = ConfigurationOptions.Parse(connString!);

            options.AbortOnConnectFail = false;

            ThreadPool.SetMinThreads(200, 200);

            return ConnectionMultiplexer.Connect(options);
        });

        services.AddHttpClient();
        services.Configure<PikudPollerOptions>(
            context.Configuration.GetSection("PikudPoller"));
        
        var useSimulatedAlerts = context.Configuration.GetValue<bool>("PikudPoller:UseSimulatedAlerts");

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