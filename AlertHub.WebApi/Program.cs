using AlertHub.Api.Options;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RedisOptions>(
    builder.Configuration.GetSection("Redis"));

builder.Services.AddControllers();
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(
        Environment.GetEnvironmentVariable("REDIS_CONNECTION") ??
                builder.Configuration["Redis:ConnectionString"]!));

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();