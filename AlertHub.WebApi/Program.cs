using AlertHub.Api.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.webapi.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:5173",
                           "https://zealous-wave-0f5f18610.6.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));

var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? 
    throw new ArgumentNullException("Redis connection string not found in appsettings.json");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnectionString);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString,
              name: "Redis",
              failureStatus: HealthStatus.Degraded);
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseRouting();
app.UseCors("AllowReactApp");
app.MapHealthChecks("/health");
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/health"));
app.Run();