using MediatR;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Domain.Products;

namespace SwarmDemo.Application.Products.Commands.DeleteProduct;

public sealed class DeleteProductCommandHandler(IProductsRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
