using SwarmDemo.Application.Products.Dtos;

namespace SwarmDemo.Application.Common.Abstractions;

public interface IProductsCache
{
    Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task SetAsync(ProductDto product, CancellationToken ct = default);
    Task RemoveAsync(Guid id, CancellationToken ct = default);
}
