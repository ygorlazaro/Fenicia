using System.Net.Mime;
using MediatR;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Queries;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

/// <summary>
///     Controller responsible for handling product category-related HTTP endpoints.
///     Provides endpoints to retrieve, create, update, and delete product categories.
/// </summary>
/// <remarks>
///     All endpoints require authentication. Categories are used to organize products.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductCategoryController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of all product categories.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number for pagination (default: 1).</param>
    /// <param name="perPage">Number of items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing the list of product categories.</returns>
    /// <response code="200">Returns the list of categories successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllProductCategoryResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllProductCategoryResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var productCategory = await sender.Send(new GetAllProductCategoryQuery(page, perPage), ct);

            return Ok(productCategory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a specific product category by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product category.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product category details if found, otherwise NotFound.</returns>
    /// <response code="200">Returns the category successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Category not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetProductCategoryByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetProductCategoryByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var productCategory = await sender.Send(new GetProductCategoryByIdQuery(id), ct);

            return productCategory is null ? NotFound() : Ok(productCategory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new product category.
    /// </summary>
    /// <param name="command">The command containing category details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created category with its details.</returns>
    /// <response code="201">Category created successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddProductCategoryResponse>> PostAsync([FromBody] AddProductCategoryCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var productCategory = await sender.Send(command, ct);

            return new CreatedResult(string.Empty, productCategory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing product category.
    /// </summary>
    /// <param name="command">The command containing updated category details.</param>
    /// <param name="id">The unique identifier of the category to update.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated category if found, otherwise NotFound.</returns>
    /// <response code="200">Category updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Category not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateProductCategoryResponse>> PatchAsync([FromBody] UpdateProductCategoryCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var productCategory = await sender.Send(command with { Id = id }, ct);

            return productCategory is null ? NotFound() : Ok(productCategory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Deletes a product category (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the category to delete.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful deletion.</returns>
    /// <response code="204">Category deleted successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await sender.Send(new DeleteProductCategoryCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves products belonging to a specific category.
    /// </summary>
    /// <param name="categoryId">The unique identifier of the category.</param>
    /// <param name="query">Pagination query parameters.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of products in the specified category.</returns>
    /// <response code="200">Returns the products successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}/product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetProductsByCategoryIdResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetProductsByCategoryIdResponse>>> GetProductsByCategoryAsync([FromRoute] Guid categoryId, [FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var products = await sender.Send(new GetProductsByCategoryIdQuery(categoryId, query.Page, query.PerPage), ct);

            return Ok(products);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}