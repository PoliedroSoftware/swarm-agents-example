using FluentValidation.TestHelper;
using SwarmDemo.Application.Products.Commands.CreateProduct;

namespace SwarmDemo.Application.Tests.Products.Commands;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Valid_command_passes()
    {
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, 10m, "USD", 5);
        _validator.TestValidate(cmd).ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc-123")]
    [InlineData("ABC_123")]
    public void Invalid_sku_fails(string sku)
    {
        var cmd = new CreateProductCommand(sku, "Demo", null, 10m, "USD", 5);
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(c => c.Sku);
    }

    [Fact]
    public void Negative_price_fails()
    {
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, -1m, "USD", 5);
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(c => c.PriceAmount);
    }

    [Fact]
    public void Wrong_currency_length_fails()
    {
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, 10m, "US", 5);
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(c => c.PriceCurrency);
    }

    [Fact]
    public void Negative_stock_fails()
    {
        var cmd = new CreateProductCommand("PROD-1", "Demo", null, 10m, "USD", -1);
        _validator.TestValidate(cmd).ShouldHaveValidationErrorFor(c => c.StockQuantity);
    }
}
