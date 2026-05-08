using MediatR;
using SwarmDemo.Application.Products.Dtos;

namespace SwarmDemo.Application.Products.Queries.ListProducts;

public sealed record ListProductsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<ProductDto>>;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int PageNumber, int PageSize, int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
