using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Supplier.Add;
using Fenicia.Module.Basic.Domains.Supplier.Delete;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;
using Fenicia.Module.Basic.Domains.Supplier.GetById;
using Fenicia.Module.Basic.Domains.Supplier.GetSupplierPerformance;
using Fenicia.Module.Basic.Domains.Supplier.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SupplierController(
    GetAllSupplierHandler getAllSupplierHandler,
    GetSupplierByIdHandler getSupplierByIdHandler,
    AddSupplierHandler addSupplierHandler,
    UpdateSupplierHandler updateSupplierHandler,
    DeleteSupplierHandler deleteSupplierHandler,
    GetSupplierPerformanceHandler getSupplierPerformanceHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllSupplierResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllSupplierResponse>>>> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken ct = default)
    {
        var suppliers = await getAllSupplierHandler.Handle(new GetAllSupplierQuery(page, perPage), ct);

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSupplierByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSupplierByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var supplier = await getSupplierByIdHandler.Handle(new GetSupplierByIdQuery(id), ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddSupplierResponse>> PostAsync([FromBody] AddSupplierCommand command, CancellationToken ct)
    {
        var supplier = await addSupplierHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, supplier);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateSupplierResponse>> PatchAsync(
        [FromBody] UpdateSupplierCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var supplier = await updateSupplierHandler.Handle(command with { Id = id }, ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteSupplierHandler.Handle(new DeleteSupplierCommand(id), ct);

        return NoContent();
    }

    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierPerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupplierPerformanceResponse>> GetPerformanceAsync(
        [FromQuery] int days = 90,
        [FromQuery] int topLimit = 10,
        CancellationToken ct = default)
    {
        var performance = await getSupplierPerformanceHandler.Handle(new GetSupplierPerformanceQuery(days, topLimit), ct);

        return Ok(performance);
    }
}
