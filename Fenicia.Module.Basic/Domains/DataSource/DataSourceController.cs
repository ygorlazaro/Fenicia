using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DataSourceController(ISender sender) : ControllerBase
{

    [HttpGet("position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllPositionForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllPositionForDataSourceResponse>>> GetPositionsAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var positions = await sender.Send(new GetAllPositionForDataSourceQuery(), ct);

            return Ok(positions);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("productcategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductCategoryForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllProductCategoryForDataSourceResponse>>> GetProductCategoriesAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var categories = await sender.Send(new GetAllProductCategoryForDataSourceQuery(), ct);

            return Ok(categories);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("supplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllSupplierForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllSupplierForDataSourceResponse>>> GetSuppliersAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var suppliers = await sender.Send(new GetAllSupplierForDataSourceQuery(), ct);

            return Ok(suppliers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCustomerForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllCustomerForDataSourceResponse>>> GetCustomersAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var customers = await sender.Send(new GetAllCustomerForDataSourceQuery(), ct);

            return Ok(customers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllProductForDataSourceResponse>>> GetProductsAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var products = await sender.Send(new GetAllProductForDataSourceQuery(), ct);

            return Ok(products);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllEmployeeForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetAllEmployeeForDataSourceResponse>>> GetEmployeesAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await sender.Send(new GetAllEmployeeForDataSourceQuery(), ct);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
