using MediatR;
using Microsoft.AspNetCore.Mvc;
using SwarmDemo.Api.Contracts.Products;
using SwarmDemo.Application.Products.Commands.CreateProduct;
using SwarmDemo.Application.Products.Commands.DeleteProduct;
using SwarmDemo.Application.Products.Commands.UpdateProduct;
using SwarmDemo.Application.Products.Dtos;
using SwarmDemo.Application.Products.Queries.GetProductById;
using SwarmDemo.Application.Products.Queries.ListProducts;

namespace SwarmDemo.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new ListProductsQuery(pageNumber, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetProductByIdQuery(id), ct);
        return Ok(ToResponse(dto));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request, CancellationToken ct)
    {
        var id = await sender.Send(new CreateProductCommand(
            request.Sku,
            request.Name,
            request.Description,
            request.PriceAmount,
            request.PriceCurrency,
            request.StockQuantity), ct);

        var dto = await sender.Send(new GetProductByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ToResponse(dto));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request, CancellationToken ct)
    {
        await sender.Send(new UpdateProductCommand(
            id,
            request.Name,
            request.Description,
            request.PriceAmount,
            request.PriceCurrency,
            request.StockQuantity), ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCommand(id), ct);
        return NoContent();
    }

    private static ProductResponse ToResponse(ProductDto dto) => new(
        dto.Id,
        dto.Sku,
        dto.Name,
        dto.Description,
        dto.PriceAmount,
        dto.PriceCurrency,
        dto.StockQuantity,
        dto.CreatedAt,
        dto.UpdatedAt);
}
