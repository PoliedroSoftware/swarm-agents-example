namespace SwarmDemo.Api.Contracts.Products;

public sealed record ProductResponse(
    Guid Id,
    string Sku,
    string Name,
    string? Description,
    decimal PriceAmount,
    string PriceCurrency,
    int StockQuantity,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
