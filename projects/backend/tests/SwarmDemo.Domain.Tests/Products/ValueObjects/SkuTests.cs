using FluentAssertions;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Domain.Tests.Products.ValueObjects;

public class SkuTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_value_is_empty(string value)
    {
        var act = () => Sku.Create(value);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_throws_when_value_exceeds_32_chars()
    {
        var act = () => Sku.Create(new string('A', 33));
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("abc-123")]
    [InlineData("ABC_123")]
    [InlineData("ABC 123")]
    [InlineData("ABC@123")]
    public void Create_throws_when_value_has_invalid_chars(string value)
    {
        var act = () => Sku.Create(value);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("ABC-123")]
    [InlineData("PRODUCT-001")]
    [InlineData("X")]
    [InlineData("12345")]
    public void Create_succeeds_for_valid_value(string value)
    {
        var sku = Sku.Create(value);
        sku.Value.Should().Be(value);
    }

    [Fact]
    public void Two_skus_with_same_value_are_equal()
    {
        var a = Sku.Create("PROD-1");
        var b = Sku.Create("PROD-1");
        a.Should().Be(b);
        (a == b).Should().BeTrue();
    }
}
