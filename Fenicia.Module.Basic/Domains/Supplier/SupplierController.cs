using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SupplierController(ISupplierService supplierService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var suppliers = await supplierService.GetAllAsync(ct);

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var supplier = await supplierService.GetByIdAsync(id, ct);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] SupplierRequest request, CancellationToken ct)
    {
        var supplier = await supplierService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, supplier);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] SupplierRequest request, [FromRoute] Guid id, CancellationToken ct)
    {
        var supplier = await supplierService.UpdateAsync(request, ct);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await supplierService.DeleteAsync(id, ct);

        return NoContent();
    }
}
