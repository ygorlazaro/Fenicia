using System.Net.Mime;

using Fenicia.Module.Basic.Domains.Supplier.Add;
using Fenicia.Module.Basic.Domains.Supplier.Delete;
using Fenicia.Module.Basic.Domains.Supplier.GetAll;
using Fenicia.Module.Basic.Domains.Supplier.GetById;
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
    DeleteSupplierHandler deleteSupplierHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SupplierResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupplierResponse>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var suppliers = await getAllSupplierHandler.Handle(new GetAllSupplierQuery(page, perPage), ct);

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupplierResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var supplier = await getSupplierByIdHandler.Handle(new GetSupplierByIdQuery(id), ct);

        return supplier is null ? NotFound() : Ok(supplier);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<SupplierResponse>> PostAsync([FromBody] AddSupplierCommand command, CancellationToken ct)
    {
        var supplier = await addSupplierHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, supplier);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<SupplierResponse>> PatchAsync(
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
    public async Task<ActionResult<SupplierResponse>> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteSupplierHandler.Handle(new DeleteSupplierCommand(id), ct);

        return NoContent();
    }
}
