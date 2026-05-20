using System.Net.Mime;
using MediatR;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Queries;
using Fenicia.Module.Basic.Domains.Supplier.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Supplier;

/// <summary>
///     Controller responsible for handling supplier-related HTTP endpoints.
///     Provides endpoints to retrieve, create, update, and delete suppliers, as well as performance analytics.
/// </summary>
/// <remarks>
///     All endpoints require authentication. Suppliers provide products to the business.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SupplierController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of all suppliers.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number for pagination (default: 1).</param>
    /// <param name="perPage">Number of items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing the list of suppliers.</returns>
    /// <response code="200">Returns the list of suppliers successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllSupplierResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllSupplierResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var suppliers = await sender.Send(new GetAllSupplierQuery(page, perPage), ct);

            return Ok(suppliers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a specific supplier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The supplier details if found, otherwise NotFound.</returns>
    /// <response code="200">Returns the supplier successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Supplier not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSupplierByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSupplierByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var supplier = await sender.Send(new GetSupplierByIdQuery(id), ct);

            return supplier is null ? NotFound() : Ok(supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new supplier.
    /// </summary>
    /// <param name="command">The command containing supplier details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created supplier with its details.</returns>
    /// <response code="201">Supplier created successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddSupplierResponse>> PostAsync([FromBody] AddSupplierCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var supplier = await sender.Send(command, ct);

            return new CreatedResult(string.Empty, supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing supplier.
    /// </summary>
    /// <param name="command">The command containing updated supplier details.</param>
    /// <param name="id">The unique identifier of the supplier to update.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated supplier if found, otherwise NotFound.</returns>
    /// <response code="200">Supplier updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Supplier not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateSupplierResponse>> PatchAsync([FromBody] UpdateSupplierCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var supplier = await sender.Send(command with { Id = id }, ct);

            return supplier is null ? NotFound() : Ok(supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Deletes a supplier (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to delete.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful deletion.</returns>
    /// <response code="204">Supplier deleted successfully.</response>
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

            await sender.Send(new DeleteSupplierCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves supplier performance analytics including summaries, product counts, and stock movements.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topLimit">Number of top suppliers to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Performance analytics including summaries, product counts, and stock movements.</returns>
    /// <response code="200">Returns performance data successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierPerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<SupplierPerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await sender.Send(new GetSupplierPerformanceQuery(days, topLimit), ct);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}