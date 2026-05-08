using FluentAssertions;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Domain.Tests.Products;

public class ProductTests
{
    private static readonly Sku Sku = Sku.Create("PROD-1");
    private static readonly Money Price = Money.Create(10m, "USD");

    [Fact]
    public void Create_throws_when_name_is_empty()
    {
        var act = () => Product.Create(Sku, "", null, Price, 0);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_throws_when_name_exceeds_200_chars()
    {
        var act = () => Product.Create(Sku, new string('a', 201), null, Price, 0);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_throws_when_stock_is_negative()
    {
        var act = () => Product.Create(Sku, "Demo", null, Price, -1);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_sets_all_properties_and_timestamps()
    {
        var p = Product.Create(Sku, "Demo", "desc", Price, 5);

        p.Id.Should().NotBeEmpty();
        p.Sku.Should().Be(Sku);
        p.Name.Should().Be("Demo");
        p.Description.Should().Be("desc");
        p.Price.Should().Be(Price);
        p.StockQuantity.Should().Be(5);
        p.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(2));
        p.UpdatedAt.Should().Be(p.CreatedAt);
    }

    [Fact]
    public void Update_changes_fields_and_bumps_UpdatedAt()
    {
        var p = Product.Create(Sku, "Demo", null, Price, 5);
        var createdAt = p.CreatedAt;
        Thread.Sleep(10);

        var newPrice = Money.Create(20m, "EUR");
        p.Update("Demo 2", "new desc", newPrice, 10);

        p.Name.Should().Be("Demo 2");
        p.Description.Should().Be("new desc");
        p.Price.Should().Be(newPrice);
        p.StockQuantity.Should().Be(10);
        p.UpdatedAt.Should().BeAfter(createdAt);
        p.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void Update_throws_when_name_is_empty()
    {
        var p = Product.Create(Sku, "Demo", null, Price, 0);
        var act = () => p.Update("", null, Price, 0);
        act.Should().Throw<ArgumentException>();
    }
}
