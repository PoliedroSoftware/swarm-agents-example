using FluentAssertions;
using Moq;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Common.Exceptions;
using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Application.Products.Queries.GetProductById;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Tests.Products.Queries;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductsRepository> _repo = new();
    private readonly Mock<IProductsCache> _cache = new();

    [Fact]
    public async Task Returns_cached_dto_when_present()
    {
        var id = Guid.NewGuid();
        var cached = new ProductDto(id, "P", "n", null, 1m, "USD", 1, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
        _cache.Setup(c => c.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(cached);

        var handler = new GetProductByIdQueryHandler(_repo.Object, _cache.Object);
        var result = await handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        result.Should().Be(cached);
        _repo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _cache.Verify(c => c.SetAsync(It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Loads_from_repo_and_caches_when_cache_miss()
    {
        var product = Product.Create(Sku.Create("P-1"), "Demo", null, Money.Create(1m, "USD"), 1);
        _cache.Setup(c => c.GetAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);

        var handler = new GetProductByIdQueryHandler(_repo.Object, _cache.Object);
        var result = await handler.Handle(new GetProductByIdQuery(product.Id), CancellationToken.None);

        result.Id.Should().Be(product.Id);
        result.Sku.Should().Be("P-1");
        _cache.Verify(c => c.SetAsync(It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Throws_NotFound_when_product_missing()
    {
        var id = Guid.NewGuid();
        _cache.Setup(c => c.GetAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((ProductDto?)null);
        _repo.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Product?)null);

        var handler = new GetProductByIdQueryHandler(_repo.Object, _cache.Object);
        var act = () => handler.Handle(new GetProductByIdQuery(id), CancellationToken.None);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
