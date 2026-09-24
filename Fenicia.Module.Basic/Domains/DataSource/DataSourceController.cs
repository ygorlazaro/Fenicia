using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.DataSource;

/// <summary>
/// Controller responsible for handling data source operations (dropdowns, lookups).
/// </summary>
/// <param name="dataSourceService">The data source service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DataSourceController(IDataSourceService dataSourceService) : ControllerBase
{
    /// <summary>
    /// Gets the list of positions for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of positions.</returns>
    /// <response code="200">List of positions returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PositionDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<PositionDataSourceResponse>>> GetPositionsAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var positions = await dataSourceService.GetPositionsAsync(cancellationToken);

            return Ok(positions);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of product categories for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of product categories.</returns>
    /// <response code="200">List of product categories returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("productcategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductCategoryDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductCategoryDataSourceResponse>>> GetProductCategoriesAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var categories = await dataSourceService.GetProductCategoriesAsync(cancellationToken);

            return Ok(categories);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of suppliers for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of suppliers.</returns>
    /// <response code="200">List of suppliers returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("supplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SupplierDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<SupplierDataSourceResponse>>> GetSuppliersAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var suppliers = await dataSourceService.GetSuppliersAsync(cancellationToken);

            return Ok(suppliers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of customers for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of customers.</returns>
    /// <response code="200">List of customers returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CustomerDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CustomerDataSourceResponse>>> GetCustomersAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var customers = await dataSourceService.GetCustomersAsync(cancellationToken);

            return Ok(customers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of products for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of products.</returns>
    /// <response code="200">List of products returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProductDataSourceResponse>>> GetProductsAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var products = await dataSourceService.GetProductsAsync(cancellationToken);

            return Ok(products);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of products for the POS, with stock and sales price.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of products with stock and price.</returns>
    /// <response code="200">List of products returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("dashboard/product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<DashboardProductDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<DashboardProductDataSourceResponse>>> GetDashboardProductsAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var products = await dataSourceService.GetDashboardProductsAsync(cancellationToken);

            return Ok(products);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the list of employees for data source.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of employees.</returns>
    /// <response code="200">List of employees returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<EmployeeDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<EmployeeDataSourceResponse>>> GetEmployeesAsync(
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await dataSourceService.GetEmployeesAsync(cancellationToken);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}