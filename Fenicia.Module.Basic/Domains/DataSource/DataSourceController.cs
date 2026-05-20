using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.DataSource;

/// <summary>
///     Controller responsible for handling datasource-related HTTP endpoints.
///     Provides read-only endpoints for retrieving lists of entities for dropdowns and data sources.
/// </summary>
/// <remarks>
///     All endpoints require authentication. Returns ordered lists of entities for use in UI selection components.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DataSourceController(ISender sender) : ControllerBase
{
    /// <summary>
    ///     Retrieves a list of all positions ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of positions.</returns>
    /// <response code="200">Returns the list of positions successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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

    /// <summary>
    ///     Retrieves a list of all product categories ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of product categories.</returns>
    /// <response code="200">Returns the list of product categories successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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

    /// <summary>
    ///     Retrieves a list of all suppliers ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of suppliers.</returns>
    /// <response code="200">Returns the list of suppliers successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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

    /// <summary>
    ///     Retrieves a list of all customers ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of customers.</returns>
    /// <response code="200">Returns the list of customers successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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

    /// <summary>
    ///     Retrieves a list of all products ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of products.</returns>
    /// <response code="200">Returns the list of products successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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

    /// <summary>
    ///     Retrieves a list of all employees ordered by name.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of employees.</returns>
    /// <response code="200">Returns the list of employees successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
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
