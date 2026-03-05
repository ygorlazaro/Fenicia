using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DataSourceController(
    GetAllPositionForDataSourceHandler getAllPositionForDataSourceHandler,
    GetAllProductCategoryForDataSourceHandler getAllProductCategoryForDataSourceHandler,
    GetAllSupplierForDataSourceHandler getAllSupplierForDataSourceHandler) : ControllerBase
{
    [HttpGet("position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllPositionForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllPositionForDataSourceResponse>>> GetPositionsAsync(CancellationToken ct)
    {
        var positions = await getAllPositionForDataSourceHandler.Handle(ct);

        return Ok(positions);
    }

    [HttpGet("productcategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductCategoryForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductCategoryForDataSourceResponse>>> GetProductCategoriesAsync(CancellationToken ct)
    {
        var categories = await getAllProductCategoryForDataSourceHandler.Handle(ct);

        return Ok(categories);
    }

    [HttpGet("supplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllSupplierForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllSupplierForDataSourceResponse>>> GetSuppliersAsync(CancellationToken ct)
    {
        var suppliers = await getAllSupplierForDataSourceHandler.Handle(ct);

        return Ok(suppliers);
    }
}
