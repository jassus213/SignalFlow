namespace SignalFlow.Example.Cache.Abstractions;

public interface ICacheService : ICacheProvider, IDisposable
{
    Task AddCacheAsync<TEntity>(string key, TEntity value, TimeSpan? ttl = null,
        CancellationToken cancellationToken = default);

    
}