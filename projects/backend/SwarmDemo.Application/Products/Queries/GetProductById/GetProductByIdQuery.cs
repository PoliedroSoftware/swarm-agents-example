using MediatR;
using SwarmDemo.Application.Products.Dtos;

namespace SwarmDemo.Application.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
