using FluentAssertions;
using Moq;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Commands.CreateProduct;
using SwarmDemo.Domain.Products;

namespace SwarmDemo.Application.Tests.Products.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductsRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task Handle_creates_product_and_returns_id()
    {
        _repo.Setup(r => r.ExistsBySkuAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync(false);

        var handler = new CreateProductCommandHandler(_repo.Object, _uow.Object);
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, 10m, "USD", 5);

        var id = await handler.Handle(cmd, CancellationToken.None);

        id.Should().NotBeEmpty();
        _repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_throws_conflict_when_sku_exists()
    {
        _repo.Setup(r => r.ExistsBySkuAsync("PROD-1", It.IsAny<CancellationToken>()))
             .ReturnsAsync(true);

        var handler = new CreateProductCommandHandler(_repo.Object, _uow.Object);
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, 10m, "USD", 5);

        var act = () => handler.Handle(cmd, CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>();
        _repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
