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
public class CustomerController(
    GetAllCustomerHandler getAllCustomerHandler,
    GetCustomerByIdHandler getCustomerByIdHandler,
    AddCustomerHandler addCustomerHandler,
    UpdateCustomerHandler updateCustomerHandler,
    DeleteCustomerHandler deleteCustomerHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var customers = await getAllCustomerHandler.Handle(new GetAllCustomerQuery(page, perPage), ct);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var customer = await getCustomerByIdHandler.Handle(new GetCustomerByIdQuery(id), ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddCustomerCommand command, CancellationToken ct)
    {
        var customer = await addCustomerHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, customer);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync(
        [FromBody] UpdateCustomerCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var customer = await updateCustomerHandler.Handle(command with { Id = id }, ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteCustomerHandler.Handle(new DeleteCustomerCommand(id), ct);

        return NoContent();
    }
}
