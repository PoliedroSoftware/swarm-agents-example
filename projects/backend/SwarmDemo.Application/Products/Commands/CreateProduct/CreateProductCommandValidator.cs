using FluentValidation;

namespace SwarmDemo.Application.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(c => c.Sku)
            .NotEmpty()
            .MaximumLength(32)
            .Matches("^[A-Z0-9-]+$");

        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(c => c.Description)
            .MaximumLength(2000);

        RuleFor(c => c.PriceAmount)
            .GreaterThanOrEqualTo(0);

        RuleFor(c => c.PriceCurrency)
            .NotEmpty()
            .Length(3);

        RuleFor(c => c.StockQuantity)
            .GreaterThanOrEqualTo(0);
    }
}
