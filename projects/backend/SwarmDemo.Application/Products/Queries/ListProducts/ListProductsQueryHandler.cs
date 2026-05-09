using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Products.Dtos;

namespace SwarmDemo.Application.Products.Queries.ListProducts;

public sealed class ListProductsQueryHandler(
    IProductsRepository repository,
    IProductsCache cache)
    : IRequestHandler<ListProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(ListProductsQuery request, CancellationToken ct)
    {
        var page = Math.Max(request.PageNumber, 1);
        var size = Math.Clamp(request.PageSize, 1, 100);

        var cached = await cache.GetListAsync(page, size, ct);
        if (cached is not null) return cached;

        var (products, total) = await repository.ListAsync(page, size, ct);

        var items = products.Select(p => new ProductDto(
            p.Id,
            p.Sku.Value,
            p.Name,
            p.Description,
            p.Price.Amount,
            p.Price.Currency,
            p.StockQuantity,
            p.CreatedAt,
            p.UpdatedAt)).ToList();

        var result = new PagedResult<ProductDto>(items, page, size, total);
        await cache.SetListAsync(result, page, size, ct);
        return result;
    }
}
