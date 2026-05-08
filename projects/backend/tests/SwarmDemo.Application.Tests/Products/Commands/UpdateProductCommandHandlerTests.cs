using FluentAssertions;
using Moq;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Commands.UpdateProduct;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Tests.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductsRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IProductsCache> _cache = new();

    [Fact]
    public async Task Handle_updates_product_and_invalidates_cache()
    {
        var product = Product.Create(Sku.Create("P-1"), "Old", null, Money.Create(1m, "USD"), 1);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new UpdateProductCommandHandler(_repo.Object, _uow.Object, _cache.Object);
        var cmd = new UpdateProductCommand(product.Id, "New", "desc", 5m, "EUR", 10);

        await handler.Handle(cmd, CancellationToken.None);

        product.Name.Should().Be("New");
        product.Description.Should().Be("desc");
        product.Price.Should().Be(Money.Create(5m, "EUR"));
        product.StockQuantity.Should().Be(10);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _cache.Verify(c => c.RemoveAsync(product.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_throws_NotFound_when_product_missing()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new UpdateProductCommandHandler(_repo.Object, _uow.Object, _cache.Object);
        var cmd = new UpdateProductCommand(id, "x", null, 1m, "USD", 1);

        var act = () => handler.Handle(cmd, CancellationToken.None);
        await act.Should().ThrowAsync<NotFoundException>();
        _cache.Verify(c => c.RemoveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
