using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler(
    IProductsRepository repository,
    IUnitOfWork unitOfWork,
    IProductsCache cache)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        if (await repository.ExistsBySkuAsync(request.Sku, ct))
            throw new ConflictException($"Product with SKU '{request.Sku}' already exists.");

        var product = Product.Create(
            Sku.Create(request.Sku),
            request.Name,
            request.Description,
            Money.Create(request.PriceAmount, request.PriceCurrency),
            request.StockQuantity);

        await repository.AddAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);

        await cache.RemoveListAsync(ct);

        return product.Id;
    }
}
