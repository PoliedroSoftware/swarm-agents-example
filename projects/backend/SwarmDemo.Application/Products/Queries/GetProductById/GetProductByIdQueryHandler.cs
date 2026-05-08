using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Domain.Products;

namespace SwarmDemo.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(
    IProductsRepository repository,
    IProductsCache cache)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var cached = await cache.GetAsync(request.Id, ct);
        if (cached is not null) return cached;

        var product = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        var dto = new ProductDto(
            product.Id,
            product.Sku.Value,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt);

        await cache.SetAsync(dto, ct);
        return dto;
    }
}
