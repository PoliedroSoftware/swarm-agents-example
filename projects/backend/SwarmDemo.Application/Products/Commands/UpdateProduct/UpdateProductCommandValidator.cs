using FluentValidation;

namespace SwarmDemo.Application.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
        RuleFor(c => c.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Description).MaximumLength(2000);
        RuleFor(c => c.PriceAmount).GreaterThanOrEqualTo(0);
        RuleFor(c => c.PriceCurrency).NotEmpty().Length(3);
        RuleFor(c => c.StockQuantity).GreaterThanOrEqualTo(0);
    }
}
