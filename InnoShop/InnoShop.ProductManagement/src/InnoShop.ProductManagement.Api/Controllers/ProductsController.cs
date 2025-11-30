using InnoShop.ProductManagement.Application.Products.Commands.CreateProduct;
using InnoShop.ProductManagement.Application.Products.Commands.DeleteProduct;
using InnoShop.ProductManagement.Application.Products.Commands.UpdateProduct;
using InnoShop.ProductManagement.Application.Products.Queries.GetProduct;
using InnoShop.ProductManagement.Application.Products.Queries.ListProducts;
using InnoShop.ProductManagement.Contracts.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.ProductManagement.Api.Controllers;

[Route("api/v1/products")]
[Authorize]
public class ProductsController(ISender mediator) : ApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateProduct(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(
            request.Title,
            request.Description,
            request.Price,
            request.ImageUrls);

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            product => CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product),
            Problem);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            product => Ok(product),
            Problem);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] Guid? sellerId = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isAvailable = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ListProductsQuery(
            page,
            pageSize,
            searchTerm,
            minPrice,
            maxPrice,
            sellerId,
            categoryId,
            isAvailable,
            sortBy,
            sortOrder);
        var result = await mediator.Send(query, cancellationToken);

        return result.Match(
            pagedResult => Ok(pagedResult),
            Problem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(
            id,
            request.Title,
            request.Description,
            request.Price);

        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            product => Ok(product),
            Problem);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
}