using Microsoft.EntityFrameworkCore;
using SignalFlow.Example.Cache;
using SignalFlow.Example.Cache.Abstractions;
using SignalFlow.Example.Infrastructure.Dal;
using SignalFlow.Example.Infrastructure.Dal.Postgres;
using SignalFlow.Example.Signals.Cache;
using SignalFlow.Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["POSTGRES_CONNECTION_STRING"]!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEventHub((configuration => { configuration.Register<AddProductCacheSignal>(); }));

builder.Services.AddSingleton<RedisCacheService>();
builder.Services.AddSingleton<InMemoryCacheService>();

builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
builder.Services.AddScoped<IProductManager, ProductManager>();

builder.Services.AddDbContext<ProductContext>(x => x.UseNpgsql(connectionString));

builder.Services.AddSingleton<ICacheProvider, CacheProvider>(x =>
    new CacheProvider(x.GetRequiredService<InMemoryCacheService>(),
        x.GetRequiredService<RedisCacheService>()));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var redisHost = builder.Configuration["REDIS_HOST"];
    var redisPort = builder.Configuration["REDIS_PORT"];
    var redisPassword = builder.Configuration["REDIS_PASSWORD"];

    var redisConnectionString = $"{redisHost}:{redisPort}";

    if (string.IsNullOrEmpty(redisConnectionString))
        throw new Exception("Configuration is missing");

    return ConnectionMultiplexer.Connect(redisConnectionString, options =>
    {
        options.Password = redisPassword;
        options.ConnectRetry = 3;
        options.ConnectTimeout = 1000;
        options.ReconnectRetryPolicy = new LinearRetry(1000);
    });
});

var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();