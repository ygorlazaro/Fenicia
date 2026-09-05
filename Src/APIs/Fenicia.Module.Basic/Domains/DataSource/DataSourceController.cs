using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.DataSource;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DataSourceController(IDataSourceService dataSourceService) : ControllerBase
{
    /// <summary>
    ///     Obtém a lista de cargos para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de cargos</returns>
    /// <response code="200">Lista de cargos retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os cargos</exception>
    [HttpGet("position")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllPositionForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllPositionForDataSourceResponse>>> GetPositionsAsync(
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
    ///     Obtém a lista de categorias de produtos para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de categorias de produtos</returns>
    /// <response code="200">Lista de categorias retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar as categorias</exception>
    [HttpGet("productcategory")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductCategoryForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductCategoryForDataSourceResponse>>> GetProductCategoriesAsync(
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
    ///     Obtém a lista de fornecedores para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de fornecedores</returns>
    /// <response code="200">Lista de fornecedores retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os fornecedores</exception>
    [HttpGet("supplier")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllSupplierForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllSupplierForDataSourceResponse>>> GetSuppliersAsync(
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
    ///     Obtém a lista de clientes para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de clientes</returns>
    /// <response code="200">Lista de clientes retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os clientes</exception>
    [HttpGet("customer")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllCustomerForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllCustomerForDataSourceResponse>>> GetCustomersAsync(
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
    ///     Obtém a lista de produtos para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de produtos</returns>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os produtos</exception>
    [HttpGet("product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllProductForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllProductForDataSourceResponse>>> GetProductsAsync(
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
    ///     Obtém a lista de produtos para o PDV, com estoque e preço de venda.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de produtos com estoque e preço</returns>
    /// <response code="200">Lista de produtos retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os produtos</exception>
    [HttpGet("dashboard/product")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllDashboardProductForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllDashboardProductForDataSourceResponse>>> GetDashboardProductsAsync(
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
    ///     Obtém a lista de funcionários para datasource.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de funcionários</returns>
    /// <response code="200">Lista de funcionários retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os funcionários</exception>
    [HttpGet("employee")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllEmployeeForDataSourceResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllEmployeeForDataSourceResponse>>> GetEmployeesAsync(
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