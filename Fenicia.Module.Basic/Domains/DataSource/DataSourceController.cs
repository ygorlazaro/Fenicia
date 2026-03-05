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
    GetAllSupplierForDataSourceHandler getAllSupplierForDataSourceHandler,
    GetAllCustomerForDataSourceHandler getAllCustomerForDataSourceHandler,
    GetAllProductForDataSourceHandler getAllProductForDataSourceHandler,
    GetAllEmployeeForDataSourceHandler getAllEmployeeForDataSourceHandler) : ControllerBase
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

    [HttpGet("customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCustomerForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCustomerForDataSourceResponse>>> GetCustomersAsync(CancellationToken ct)
    {
        var customers = await getAllCustomerForDataSourceHandler.Handle(ct);

        return Ok(customers);
    }

    [HttpGet("product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductForDataSourceResponse>>> GetProductsAsync(CancellationToken ct)
    {
        var products = await getAllProductForDataSourceHandler.Handle(ct);

        return Ok(products);
    }

    [HttpGet("employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllEmployeeForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllEmployeeForDataSourceResponse>>> GetEmployeesAsync(CancellationToken ct)
    {
        var employees = await getAllEmployeeForDataSourceHandler.Handle(ct);

        return Ok(employees);
    }
}
