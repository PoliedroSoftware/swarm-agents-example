using SwarmDemo.Domain.Common;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Domain.Products;

public sealed class Product : Entity<Guid>, IAggregateRoot
{
    public Sku Sku { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Money Price { get; private set; } = default!;
    public int StockQuantity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Product() { }

    private Product(Guid id, Sku sku, string name, string? description, Money price, int stockQuantity, DateTimeOffset now)
    {
        Id = id;
        Sku = sku;
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Product Create(Sku sku, string name, string? description, Money price, int stockQuantity)
    {
        ValidateName(name);
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        return new Product(Guid.NewGuid(), sku, name, description, price, stockQuantity, DateTimeOffset.UtcNow);
    }

    public void Update(string name, string? description, Money price, int stockQuantity)
    {
        ValidateName(name);
        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));
        if (name.Length > 200)
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));
    }
}
