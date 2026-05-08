using FluentAssertions;
using Moq;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Products.Queries.ListProducts;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Application.Tests.Products.Queries;

public class ListProductsQueryHandlerTests
{
    private readonly Mock<IProductsRepository> _repo = new();

    [Fact]
    public async Task Returns_paged_dtos()
    {
        IReadOnlyList<Product> products = new List<Product>
        {
            Product.Create(Sku.Create("P-1"), "A", null, Money.Create(1m, "USD"), 1),
            Product.Create(Sku.Create("P-2"), "B", null, Money.Create(2m, "USD"), 2)
        };
        _repo.Setup(r => r.ListAsync(1, 20, It.IsAny<CancellationToken>())).ReturnsAsync((products, 5));

        var handler = new ListProductsQueryHandler(_repo.Object);
        var result = await handler.Handle(new ListProductsQuery(1, 20), CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Clamps_pagination_to_safe_bounds()
    {
        IReadOnlyList<Product> empty = new List<Product>();
        _repo.Setup(r => r.ListAsync(1, 100, It.IsAny<CancellationToken>())).ReturnsAsync((empty, 0));

        var handler = new ListProductsQueryHandler(_repo.Object);
        var result = await handler.Handle(new ListProductsQuery(0, 1000), CancellationToken.None);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(100);
        result.Items.Should().BeEmpty();
    }
}
