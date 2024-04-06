using SignalFlow.Example.Cache.Abstractions;

namespace SignalFlow.Example.Cache;

public class CacheProvider : ICacheProvider
{
    private readonly ICacheProvider _inMemoryCache;
    private readonly ICacheProvider _redisCache;

    public CacheProvider(ICacheProvider inMemoryCache, ICacheProvider redisCache)
    {
        _inMemoryCache = inMemoryCache;
        _redisCache = redisCache;
    }

    public async Task<TEntity?> GetCacheAsync<TEntity>(string key, CancellationToken cancellationToken)
    {
        var inMemoryValue = await _inMemoryCache.GetCacheAsync<TEntity>(key, cancellationToken);
        if (inMemoryValue != null)
            return inMemoryValue;

        var redisMemoryValue = await _redisCache.GetCacheAsync<TEntity>(key, cancellationToken);
        if (inMemoryValue != null)
            return redisMemoryValue;

        return default;
    }
}