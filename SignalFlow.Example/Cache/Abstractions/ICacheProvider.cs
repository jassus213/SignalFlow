namespace SignalFlow.Example.Cache.Abstractions;

public interface ICacheProvider
{
    Task<TEntity?> GetCacheAsync<TEntity>(string key, CancellationToken cancellationToken = default);
}