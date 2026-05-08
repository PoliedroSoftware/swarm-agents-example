using FluentAssertions;
using Moq;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Commands.DeleteProduct;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Tests.Products.Commands;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductsRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IProductsCache> _cache = new();

    [Fact]
    public async Task Handle_removes_product_and_invalidates_cache()
    {
        var product = Product.Create(Sku.Create("P-1"), "Demo", null, Money.Create(1m, "USD"), 1);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new DeleteProductCommandHandler(_repo.Object, _uow.Object, _cache.Object);
        await handler.Handle(new DeleteProductCommand(product.Id), CancellationToken.None);

        _repo.Verify(r => r.Remove(product), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(c => c.RemoveAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_throws_NotFound_when_product_missing()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new DeleteProductCommandHandler(_repo.Object, _uow.Object, _cache.Object);
        var act = () => handler.Handle(new DeleteProductCommand(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
        _repo.Verify(r => r.Remove(It.IsAny<Product>()), Times.Never);
        _cache.Verify(c => c.RemoveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
