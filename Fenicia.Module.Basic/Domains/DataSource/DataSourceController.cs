using System.Net.Mime;

using Fenicia.Common.API;

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
    public async Task<ActionResult<List<GetAllPositionForDataSourceResponse>>> GetPositionsAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var positions = await getAllPositionForDataSourceHandler.Handle(ct);

        return Ok(positions);
    }

    [HttpGet("productcategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductCategoryForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductCategoryForDataSourceResponse>>> GetProductCategoriesAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var categories = await getAllProductCategoryForDataSourceHandler.Handle(ct);

        return Ok(categories);
    }

    [HttpGet("supplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllSupplierForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllSupplierForDataSourceResponse>>> GetSuppliersAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var suppliers = await getAllSupplierForDataSourceHandler.Handle(ct);

        return Ok(suppliers);
    }

    [HttpGet("customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCustomerForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCustomerForDataSourceResponse>>> GetCustomersAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var customers = await getAllCustomerForDataSourceHandler.Handle(ct);

        return Ok(customers);
    }

    [HttpGet("product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductForDataSourceResponse>>> GetProductsAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var products = await getAllProductForDataSourceHandler.Handle(ct);

        return Ok(products);
    }

    [HttpGet("employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllEmployeeForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllEmployeeForDataSourceResponse>>> GetEmployeesAsync(
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var employees = await getAllEmployeeForDataSourceHandler.Handle(ct);

        return Ok(employees);
    }
}
