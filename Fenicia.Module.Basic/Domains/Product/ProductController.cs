using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Product;

/// <summary>
///     Controller responsible for handling product-related HTTP endpoints.
///     Provides endpoints to retrieve, create, update, and delete products, as well as retrieve product performance metrics.
/// </summary>
/// <remarks>
///     All endpoints require authentication. Products are associated with categories and suppliers.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductController(GetAllProductHandler getAllProductHandler, GetProductByIdHandler getProductByIdHandler, AddProductHandler addProductHandler, UpdateProductHandler updateProductHandler, DeleteProductHandler deleteProductHandler, GetProductPerformanceHandler getProductPerformanceHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of all products.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number for pagination (default: 1).</param>
    /// <param name="perPage">Number of items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing the list of products.</returns>
    /// <response code="200">Returns the list of products successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllProductResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllProductResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var products = await getAllProductHandler.Handle(new GetAllProductQuery(page, perPage), ct);

        return Ok(products);
    }

    /// <summary>
    ///     Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product details if found, otherwise NotFound.</returns>
    /// <response code="200">Returns the product successfully.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetProductByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var product = await getProductByIdHandler.Handle(new GetProductByIdQuery(id), ct);

        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    ///     Creates a new product.
    /// </summary>
    /// <param name="command">The command containing product details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created product with its details.</returns>
    /// <response code="201">Product created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProductResponse>> PostAsync([FromBody] AddProductCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var product = await addProductHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, product);
    }

    /// <summary>
    ///     Updates an existing product.
    /// </summary>
    /// <param name="command">The command containing updated product details.</param>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated product if found, otherwise NotFound.</returns>
    /// <response code="200">Product updated successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="404">Product not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProductResponse>> PatchAsync([FromBody] UpdateProductCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var product = await updateProductHandler.Handle(command with { Id = id }, ct);

        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>
    ///     Deletes a product (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful deletion.</returns>
    /// <response code="204">Product deleted successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        await deleteProductHandler.Handle(new DeleteProductCommand(id), ct);

        return NoContent();
    }

    /// <summary>
    ///     Retrieves product performance metrics including best-selling, worst-selling, and never sold products.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topLimit">Number of top/bottom products to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Performance metrics including best-selling, worst-selling, and never sold products.</returns>
    /// <response code="200">Returns product performance data successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductPerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductPerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var performance = await getProductPerformanceHandler.Handle(new GetProductPerformanceQuery(days, topLimit), ct);

        return Ok(performance);
    }
}