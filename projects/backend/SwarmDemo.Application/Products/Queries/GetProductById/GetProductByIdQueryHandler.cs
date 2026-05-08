using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Domain.Products;

namespace SwarmDemo.Application.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductsRepository repository)
    : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return new ProductDto(
            product.Id,
            product.Sku.Value,
            product.Name,
            product.Description,
            product.Price.Amount,
            product.Price.Currency,
            product.StockQuantity,
            product.CreatedAt,
            product.UpdatedAt);
    }
}
