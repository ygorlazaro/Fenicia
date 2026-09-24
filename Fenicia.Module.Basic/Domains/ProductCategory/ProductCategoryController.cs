using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

/// <summary>
/// Controller responsible for handling product category-related operations.
/// </summary>
/// <param name="productCategoryService">The product category service.</param>
/// <param name="companyContext">The company context.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductCategoryController(IProductCategoryService productCategoryService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of product categories.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of product categories.</returns>
    /// <response code="200">List of product categories returned successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<ProductCategoryResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ProductCategoryResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] ProductCategoryQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var categories = await productCategoryService.GetAllAsync(query, cancellationToken);

            return Ok(categories);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets a product category by ID.
    /// </summary>
    /// <param name="id">Product category ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Product category data.</returns>
    /// <response code="200">Product category found.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Product category not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductCategoryResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var category = await productCategoryService.GetByIdAsync(id, cancellationToken);

            return category is null ? NotFound() : Ok(category);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new product category.
    /// </summary>
    /// <param name="command">Product category data.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created product category.</returns>
    /// <response code="201">Product category created successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ProductCategoryResponse>> PostAsync(
        [FromBody] ProductCategoryRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = companyContext.CompanyId;
            var category = await productCategoryService.AddAsync(command, companyId, cancellationToken);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = category.Id }, category);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing product category.
    /// </summary>
    /// <param name="id">Product category ID.</param>
    /// <param name="command">Updated product category data.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Updated product category.</returns>
    /// <response code="200">Product category updated successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Product category not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductCategoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ProductCategoryResponse>> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] ProductCategoryRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = companyContext.CompanyId;
            var category = await productCategoryService.UpdateAsync(id, command, companyId, cancellationToken);

            return category is null ? NotFound() : Ok(category);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a product category (soft delete).
    /// </summary>
    /// <param name="id">Product category ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Product category deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = companyContext.CompanyId;
            await productCategoryService.DeleteAsync(id, companyId, cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}