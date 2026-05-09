using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using SwarmDemo.Application.Common.Abstractions;
using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Application.Products.Queries.ListProducts;

namespace SwarmDemo.Infrastructure.Caching;

public sealed class RedisProductsCache : IProductsCache
{
    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;

    private static readonly DistributedCacheEntryOptions SingleOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(5),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
    };

    private static readonly DistributedCacheEntryOptions ListOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(2),
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15)
    };

    public RedisProductsCache(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redis = redis;
    }

    public async Task<ProductDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(Key(id), ct);
        return bytes is null ? null : JsonSerializer.Deserialize<ProductDto>(bytes);
    }

    public Task SetAsync(ProductDto product, CancellationToken ct = default) =>
        _cache.SetAsync(Key(product.Id), JsonSerializer.SerializeToUtf8Bytes(product), SingleOptions, ct);

    public Task RemoveAsync(Guid id, CancellationToken ct = default) =>
        _cache.RemoveAsync(Key(id), ct);

    public async Task<PagedResult<ProductDto>?> GetListAsync(int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var bytes = await _cache.GetAsync(ListKey(pageNumber, pageSize), ct);
        return bytes is null ? null : JsonSerializer.Deserialize<PagedResult<ProductDto>>(bytes);
    }

    public Task SetListAsync(PagedResult<ProductDto> result, int pageNumber, int pageSize, CancellationToken ct = default) =>
        _cache.SetAsync(ListKey(pageNumber, pageSize), JsonSerializer.SerializeToUtf8Bytes(result), ListOptions, ct);

    public async Task RemoveListAsync(CancellationToken ct = default)
    {
        var server = _redis.GetServer(_redis.GetEndPoints()[0]);
        var keys = server.Keys(pattern: $"swarm-demo:{ListPattern}").ToArray();

        if (keys.Length > 0)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(keys);
        }
    }

    private static string Key(Guid id) => $"products:{id}";

    private static string ListKey(int page, int size) => $"products:list:page-{page}-size-{size}";

    private const string ListPattern = "products:list:*";
}
