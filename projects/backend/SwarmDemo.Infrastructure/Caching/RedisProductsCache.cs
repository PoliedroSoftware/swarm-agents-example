using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Products.Dtos;

namespace SwarmDemo.Infrastructure.Caching;

public sealed class RedisProductsCache(IDistributedCache cache) : IProductsCache
{
    private static readonly DistributedCacheEntryOptions Options = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    public async Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var bytes = await cache.GetAsync(Key(id), ct);
        return bytes is null ? null : JsonSerializer.Deserialize<ProductDto>(bytes);
    }

    public Task SetAsync(ProductDto product, CancellationToken ct = default) =>
        cache.SetAsync(Key(product.Id), JsonSerializer.SerializeToUtf8Bytes(product), Options, ct);

    public Task RemoveAsync(Guid id, CancellationToken ct = default) =>
        cache.RemoveAsync(Key(id), ct);

    private static string Key(Guid id) => $"products:{id}";
}
