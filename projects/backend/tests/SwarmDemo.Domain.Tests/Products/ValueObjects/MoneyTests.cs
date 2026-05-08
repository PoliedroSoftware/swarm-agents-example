using FluentAssertions;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Domain.Tests.Products.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_throws_when_amount_is_negative()
    {
        var act = () => Money.Create(-1m, "USD");
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_throws_when_currency_is_empty(string currency)
    {
        var act = () => Money.Create(10m, currency);
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDX")]
    public void Create_throws_when_currency_length_is_not_3(string currency)
    {
        var act = () => Money.Create(10m, currency);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_rounds_amount_to_two_decimals()
    {
        var m = Money.Create(9.999m, "USD");
        m.Amount.Should().Be(10.00m);
    }

    [Fact]
    public void Create_uppercases_currency()
    {
        var m = Money.Create(1m, "usd");
        m.Currency.Should().Be("USD");
    }

    [Fact]
    public void Same_amount_and_currency_are_equal()
    {
        Money.Create(1m, "USD").Should().Be(Money.Create(1m, "USD"));
    }

    [Fact]
    public void Different_currency_means_not_equal()
    {
        Money.Create(1m, "USD").Should().NotBe(Money.Create(1m, "EUR"));
    }
}
