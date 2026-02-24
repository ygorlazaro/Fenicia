using System.Net.Mime;

using Fenicia.Module.Basic.Domains.Customer.Add;
using Fenicia.Module.Basic.Domains.Customer.Delete;
using Fenicia.Module.Basic.Domains.Customer.GetAll;
using Fenicia.Module.Basic.Domains.Customer.GetById;
using Fenicia.Module.Basic.Domains.Customer.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Customer;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class PositionController(
    GetAllCustomerHandler getAllCustomerHandler,
    GetCustomerByIdHandler getCustomerByIdHandler,
    AddCustomerHandler addCustomerHandler,
    UpdateCustomerHandler updateCustomerHandler,
    DeleteCustomerHandler deleteCustomerHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AddCustomerResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCustomerResponse>>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var customers = await getAllCustomerHandler.Handle(new GetAllCustomerQuery(page, perPage), ct);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCustomerResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCustomerByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var customer = await getCustomerByIdHandler.Handle(new GetCustomerByIdQuery(id), ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateCustomerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCustomerResponse>> PostAsync([FromBody] AddCustomerCommand command, CancellationToken ct)
    {
        var customer = await addCustomerHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, customer);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCustomerResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCustomerResponse>> PatchAsync(
        [FromBody] UpdateCustomerCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var customer = await updateCustomerHandler.Handle(command with { Id = id }, ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteCustomerHandler.Handle(new DeleteCustomerCommand(id), ct);

        return NoContent();
    }
}
