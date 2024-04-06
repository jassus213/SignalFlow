using Microsoft.AspNetCore.Mvc;
using SignalFlow.Example.Infrastructure.Dal;

namespace SignalFlow.Example.Controllers;

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
            return BadRequest();
        }

        return Ok();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProductAsync(int id, CancellationToken cancellationToken)
    {
        var product = await _productManager.GetProductAsync(id, cancellationToken);
        if (product == null)
            return NotFound(product);

        return Ok(product);
    }
    
}


public record ProductRequest
{
    public required string Tittle { get; set; }
    public required int Cost { get; set; }
}