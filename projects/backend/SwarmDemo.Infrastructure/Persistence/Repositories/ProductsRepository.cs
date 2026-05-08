using Microsoft.EntityFrameworkCore;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Domain.Products;
using SwarmDemo.Domain.Products.ValueObjects;

namespace SwarmDemo.Infrastructure.Persistence.Repositories;

public sealed class ProductsRepository(AppDbContext context) : IProductsRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken ct = default)
    {
        var skuVo = Sku.Create(sku);
        return context.Products.AnyAsync(p => p.Sku == skuVo, ct);
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> ListAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = context.Products.AsNoTracking().OrderBy(p => p.CreatedAt);
        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Product product, CancellationToken ct = default) =>
        await context.Products.AddAsync(product, ct);

    public void Remove(Product product) => context.Products.Remove(product);
}
