//Test: change triggers deploy
using AlertHub.Api.Options;
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
                           "https://wonderful-field-0b7bb1810.1.azurestaticapps.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));

var redisConnectionString = builder.Configuration["Redis:ConnectionString"] ?? 
    throw new ArgumentNullException("Redis connection string not found in appsettings.json");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "Redis");
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AllowReactApp");

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();