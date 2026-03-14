using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Supplier.Commands;
using Fenicia.Module.Basic.Domains.Supplier.Handlers;
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
public class SupplierController(GetAllSupplierHandler getAllSupplierHandler, GetSupplierByIdHandler getSupplierByIdHandler, AddSupplierHandler addSupplierHandler, UpdateSupplierHandler updateSupplierHandler, DeleteSupplierHandler deleteSupplierHandler, GetSupplierPerformanceHandler getSupplierPerformanceHandler) : ControllerBase
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
    /// <response code="500">Internal server error.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllSupplierResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllSupplierResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var suppliers = await getAllSupplierHandler.Handle(new GetAllSupplierQuery(page, perPage), ct);

        return Ok(suppliers);
    }

    /// <summary>
    ///     Retrieves a specific supplier by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The supplier details if found, otherwise NotFound.</returns>
    /// <response code="200">Returns the supplier successfully.</response>
    /// <response code="404">Supplier not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSupplierByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSupplierByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var supplier = await getSupplierByIdHandler.Handle(new GetSupplierByIdQuery(id), ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    /// <summary>
    ///     Creates a new supplier.
    /// </summary>
    /// <param name="command">The command containing supplier details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created supplier with its details.</returns>
    /// <response code="201">Supplier created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddSupplierResponse>> PostAsync([FromBody] AddSupplierCommand command, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var supplier = await addSupplierHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, supplier);
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
    /// <response code="400">Invalid request data.</response>
    /// <response code="404">Supplier not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateSupplierResponse>> PatchAsync([FromBody] UpdateSupplierCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var supplier = await updateSupplierHandler.Handle(command with { Id = id }, ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    /// <summary>
    ///     Deletes a supplier (soft delete).
    /// </summary>
    /// <param name="id">The unique identifier of the supplier to delete.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on successful deletion.</returns>
    /// <response code="204">Supplier deleted successfully.</response>
    /// <response code="500">Internal server error.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        await deleteSupplierHandler.Handle(new DeleteSupplierCommand(id), ct);

        return NoContent();
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
    /// <response code="500">Internal server error.</response>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierPerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupplierPerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var performance = await getSupplierPerformanceHandler.Handle(new GetSupplierPerformanceQuery(days, topLimit), ct);

        return Ok(performance);
    }
}