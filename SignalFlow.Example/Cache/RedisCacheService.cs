using Newtonsoft.Json;
using SignalFlow.Abstractions;
using SignalFlow.Example.Cache.Abstractions;
using SignalFlow.Example.Infrastructure.Product;
using SignalFlow.Example.Signals.Cache;
using StackExchange.Redis;

namespace SignalFlow.Example.Cache;

public class RedisCacheService : ICacheService
{
    private readonly ISubscriptionManager _subscriptionManager;
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;

    private const string CACHE_PREFIX = "cache";

    public RedisCacheService(ISubscriptionManager subscriptionManager, IConnectionMultiplexer connectionMultiplexer,
        ILogger<RedisCacheService> logger)
    {
        _subscriptionManager = subscriptionManager;
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;

        _subscriptionManager.Subscribe<AddProductCacheSignal>(HandleCacheUpdateAsync);
    }

    public async Task AddCacheAsync<TEntity>(string key, TEntity value, TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            return;

        try
        {
            var db = _connectionMultiplexer.GetDatabase();
            var valueString = JsonConvert.SerializeObject(value);
            ttl ??= TimeSpan.FromMinutes(1);

            await db.StringSetAsync($"{CACHE_PREFIX}:{key}", valueString, ttl, flags: CommandFlags.FireAndForget);
        }
        catch (Exception e)
        {
            _logger.LogError("An exception occurred when adding the cache, key: {key} \n Exception Message: {message}",
                key, e.Message);

            throw;
        }
    }

    public async Task<TEntity?> GetCacheAsync<TEntity>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            return default;
        try
        {
            var dataBase = _connectionMultiplexer.GetDatabase();
            var redisValue = await dataBase.StringGetAsync($"{CACHE_PREFIX}:{key}");

            return redisValue.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<TEntity>(redisValue!);
        }
        catch
        {
            return default;
        }
    }

    private async Task HandleCacheUpdateAsync(AddProductCacheSignal signal)
    {
        var key = $"{CACHE_PREFIX}:{signal.Id}";

        await AddCacheAsync(key, new ProductDto
        {
            Id = signal.Id,
            Tittle = signal.Tittle,
            Cost = signal.Cost
        });
    }

    public void Dispose()
    {
        _subscriptionManager.UnSubscribe<AddProductCacheSignal>(HandleCacheUpdateAsync);
    }
}