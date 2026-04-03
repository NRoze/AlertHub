using Microsoft.Extensions.Hosting;
using AlertHub.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AlertHub.Api.Options;
using AlertHub.Api.Middleware;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerOptions =>
    {
        workerOptions.UseMiddleware<GlobalExceptionMiddleware>();
    })
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<StackExchange.Redis.IConnectionMultiplexer>(sp => 
            StackExchange.Redis.ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379"));

        services.AddHttpClient();
        services.Configure<PikudPollerOptions>(
            context.Configuration.GetSection("PikudPoller"));
        services.AddScoped<IPikudPollerService, PikudPollerService>();
        services.AddScoped<IAlertCache, AlertCache>();
    })
    .Build();


host.Run();