using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Customer;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CustomerController(ICustomerService customerService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var customers = await customerService.GetAllAsync(ct);

        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var customer = await customerService.GetByIdAsync(id, ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] CustomerRequest request, CancellationToken ct)
    {
        var customer = await customerService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, customer);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] CustomerRequest request, [FromRoute] Guid id, CancellationToken ct)
    {
        var customer = await customerService.UpdateAsync(request, ct);

        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await customerService.DeleteAsync(id, ct);

        return NoContent();
    }
}