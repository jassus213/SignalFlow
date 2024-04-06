using System.Collections.Concurrent;
using Newtonsoft.Json;
using SignalFlow.Abstractions;
using SignalFlow.Example.Cache.Abstractions;
using SignalFlow.Example.Infrastructure.Product;
using SignalFlow.Example.Signals.Cache;

namespace SignalFlow.Example.Cache;

public class InMemoryCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, (string, DateTime?)> _storage =
        new ConcurrentDictionary<string, (string, DateTime?)>();

    private readonly ISubscriptionManager _subscriptionManager;
    private readonly ILogger<InMemoryCacheService> _logger;

    private const string CACHE_PREFIX = "cache";

    public InMemoryCacheService(ISubscriptionManager subscriptionManager, ILogger<InMemoryCacheService> logger)
    {
        _subscriptionManager = subscriptionManager;
        _logger = logger;

        _subscriptionManager.Subscribe<AddProductCacheSignal>(HandleCacheUpdateAsync);
    }

    public Task AddCacheAsync<TEntity>(string key, TEntity value, TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            return Task.CompletedTask;

        ttl ??= TimeSpan.FromMinutes(1);

        var storageValue = (JsonConvert.SerializeObject(value), DateTime.UtcNow.Add(ttl.Value));
        if (!_storage.TryAdd(key, storageValue))
            _logger.LogWarning("Something went wrong when adding the cache, key {key}", key);

        return Task.CompletedTask;
    }

    public Task<TEntity?> GetCacheAsync<TEntity>(string key, CancellationToken cancellationToken = default)
    {
        if (_storage.TryGetValue($"{CACHE_PREFIX}:{key}", out var value))
        {
            return value.Item2 < DateTime.UtcNow
                ? Task.FromResult<TEntity>(default)
                : Task.FromResult(JsonConvert.DeserializeObject<TEntity>(value.Item1));
        }

        return Task.FromResult<TEntity>(default);
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