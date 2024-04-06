using Microsoft.EntityFrameworkCore;
using SignalFlow.Abstractions.Firing;
using SignalFlow.Example.Cache;
using SignalFlow.Example.Cache.Abstractions;
using SignalFlow.Example.Controllers;
using SignalFlow.Example.Infrastructure.Dal.Postgres;
using SignalFlow.Example.Infrastructure.Product;
using SignalFlow.Example.Signals.Cache;

namespace SignalFlow.Example.Infrastructure.Dal;

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