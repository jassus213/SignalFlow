using SignalFlow.Example.Controllers;
using SignalFlow.Example.Infrastructure.Product;

namespace SignalFlow.Example.Infrastructure.Dal;

public interface IProductManager
{
    Task AddProductAsync(ProductRequest request, CancellationToken cancellationToken);
    Task<ProductDto?> GetProductAsync(int id, CancellationToken cancellationToken);
}