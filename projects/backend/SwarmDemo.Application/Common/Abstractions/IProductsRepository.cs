using SwarmDemo.Domain.Products;

namespace SwarmDemo.Application.Common.Abstractions;

public interface IProductsRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken ct = default);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> ListAsync(int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    void Remove(Product product);
}
