using Fenicia.Common.Database.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SupplierController(ISupplierService supplierService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var suppliers = await supplierService.GetAllAsync(cancellationToken);

        return Ok(suppliers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.GetByIdAsync(id, cancellationToken);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] SupplierRequest request, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.AddAsync(request, cancellationToken);

        return new CreatedResult(string.Empty, supplier);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] SupplierRequest request, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var supplier = await supplierService.UpdateAsync(request, cancellationToken);

        if (supplier is null)
        {
            return NotFound();
        }

        return Ok(supplier);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await supplierService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
