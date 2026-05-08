namespace SwarmDemo.Api.Contracts.Products;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal PriceAmount,
    string PriceCurrency,
    int StockQuantity);
