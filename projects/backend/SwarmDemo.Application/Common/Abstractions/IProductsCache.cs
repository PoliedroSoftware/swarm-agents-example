using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Application.Products.Queries.ListProducts;

namespace SwarmDemo.Application.Common.Abstractions;

public interface IProductsCache
{
    Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default);
    Task SetAsync(ProductDto product, CancellationToken ct = default);
    Task RemoveAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<ProductDto>?> GetListAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task SetListAsync(PagedResult<ProductDto> result, int pageNumber, int pageSize, CancellationToken ct = default);
    Task RemoveListAsync(CancellationToken ct = default);
}
