# Using SignalFlow Framework: Example Documentation

In this example, we'll demonstrate how to use the SignalFlow framework within a .NET application that involves managing
product caches stored in both In-Memory and Redis implementations.

## Signal Definition

First, let's define a signal class AddProductCacheSignal representing a signal to add product information to the cache.

```csharp
public class AddProductCacheSignal : Signal<AddProductCacheSignal>
{
    public int Id { get; set; }
    public string Tittle { get; set; }
    public int Cost { get; set; }
}
```

## Registration Signal

Register the AddProductCacheSignal signal with SignalFlow in the Program.cs file using the AddEventHub method.

```csharp
builder.Services.AddEventHub((configuration => { configuration.Register<AddProductCacheSignal>(); }));
```

## Controller

Now, let's see how the ProductController interacts with the ProductManager to add and retrieve products.

```csharp
[ApiController]
[Route("products")]
public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IProductManager _productManager;

    public ProductController(ILogger<ProductController> logger, IProductManager productManager)
    {
        _logger = logger;
        _productManager = productManager;
    }

    [HttpPost]
    public async Task<IActionResult> AddProductAsync([FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await _productManager.AddProductAsync(request, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to add product.");
            return BadRequest();
        }

        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _productManager.GetProductAsync(id, cancellationToken);
        if (product == null)
            return NotFound();

        return Ok(product);
    }
}
```

## Product Manager Implementation

The ProductManager class handles product operations, including adding products to the database and firing cache update
signals.

```csharp
public class ProductManager : IProductManager
{
    private readonly ProductContext _productContext;
    private readonly IFireManager _fireManager;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<ProductManager> _logger;

    public ProductManager(ProductContext productContext, IFireManager fireManager, ICacheProvider cacheProvider,
        ILogger<ProductManager> logger)
    {
        _productContext = productContext;
        _fireManager = fireManager;
        _cacheProvider = cacheProvider;
        _logger = logger;
    }

    public async Task AddProductAsync(ProductRequest request, CancellationToken cancellationToken)
    {
        var product = new Product.Product
        {
            Tittle = request.Tittle,
            Cost = request.Cost
        };

        var entry = await _productContext.Products.AddAsync(product, cancellationToken);
        try
        {
            await _productContext.SaveChangesAsync(cancellationToken);
            await _fireManager.FireAsync(() => new AddProductCacheSignal()
            {
                Id = entry.Entity.Id,
                Tittle = request.Tittle,
                Cost = request.Cost
            });
        }
        catch (Exception e)
        {
            _logger.LogCritical(
                "An exception occurred when adding a Product, Tittle: {tittle} \n Exception Message: {message}",
                request.Tittle, e.Message);
            throw;
        }
    }

    public async Task<ProductDto?> GetProductAsync(int id, CancellationToken cancellationToken)
    {
        var cacheResult = await _cacheProvider.GetCacheAsync<ProductDto>(id.ToString(), cancellationToken);
        if (cacheResult != null)
            return cacheResult;

        return await _productContext.Products.Where(x => x.Id == id).Select(x => new ProductDto
        {
            Tittle = x.Tittle,
            Cost = x.Cost
        }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
    }
}
```

## Cache Implementation

### In Memory Implementation

Here's an example of an In-Memory cache service implementation subscribing to AddProductCacheSignal signals to update
the cache.

```csharp
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
```

### Redis Implementation

```csharp
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
```

## Summary

In this example, we demonstrated how to use the SignalFlow framework to manage product caches in an ASP.NET Core
application using In-Memory and Redis implementations. Signals are fired when products are added, and cache services
subscribe to these signals to update their caches accordingly.

Feel free to customize and extend this example to fit your specific use case and requirements with the SignalFlow
framework. For more information and advanced usage scenarios, refer to the framework's documentation and examples.