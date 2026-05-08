using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler(
    IProductsRepository repository,
    IUnitOfWork unitOfWork,
    IProductsCache cache)
    : IRequestHandler<UpdateProductCommand>
{
    public async Task Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.Update(
            request.Name,
            request.Description,
            Money.Create(request.PriceAmount, request.PriceCurrency),
            request.StockQuantity);

        await unitOfWork.SaveChangesAsync(ct);
        await cache.RemoveAsync(request.Id, ct);
    }
}
