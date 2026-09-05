using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Module.Basic.Domains.Customer.DTOs;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Customer;

/// <inheritdoc />
/// <summary>
///     Gerencia operações de clientes.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CustomerController(ICustomerService customerService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    ///     Obtém uma lista paginada de clientes.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Consulta avançada para filtros</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de clientes</returns>
    /// <response code="200">Lista de clientes retornada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os clientes</exception>
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<List<GetAllCustomerResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllCustomerResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customers = await customerService.GetAllAsync(
            new GetAllCustomerQuery(page, perPage, query, sort),
            cancellationToken);

        return Ok(customers);
    }

    /// <summary>
    ///     Obtém um cliente pelo ID.
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dados do cliente</returns>
    /// <response code="200">Cliente encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Cliente não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o cliente</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetCustomerByIdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetCustomerByIdResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.GetByIdAsync(new GetCustomerByIdQuery(id), cancellationToken);

        return customer is null ? NotFound() : Ok(customer);
    }

    /// <summary>
    ///     Cria um novo cliente.
    /// </summary>
    /// <param name="command">Dados do cliente a ser criado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Cliente criado</returns>
    /// <response code="201">Cliente criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AddCustomerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddCustomerResponse>> PostAsync(
        [FromBody] AddCustomerCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.AddAsync(command, companyContext.CompanyId, cancellationToken);

        return new CreatedResult(string.Empty, customer);
    }

    /// <summary>
    ///     Atualiza um cliente existente.
    /// </summary>
    /// <param name="command">Dados atualizados do cliente</param>
    /// <param name="id">ID do cliente</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Cliente atualizado</returns>
    /// <response code="200">Cliente atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Cliente não encontrado</response>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UpdateCustomerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateCustomerResponse>> PatchAsync(
        [FromBody] UpdateCustomerCommand command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var customer = await customerService.UpdateAsync(
            command with { Id = id },
            companyContext.CompanyId,
            cancellationToken);

        return customer switch
        {
            null => NotFound(),
            _ => Ok(customer)
        };
    }

    /// <summary>
    ///     Remove um cliente (soft delete).
    /// </summary>
    /// <param name="id">ID do cliente</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <response code="204">Cliente removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await customerService.DeleteAsync(new DeleteCustomerCommand(id), companyContext.CompanyId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Obtém insights agregados de clientes.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Período em dias para análise</param>
    /// <param name="topLimit">Limite de registros no top</param>
    /// <param name="riskThresholdDays">Limite de dias para considerar cliente em risco</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Insights de clientes</returns>
    /// <response code="200">Insights retornados com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os insights</exception>
    [HttpGet("insights")]
    [ProducesResponseType(typeof(CustomerInsightsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerInsightsResponse>> GetInsightsAsync(
        WideEventContext wide,
        [FromQuery] int days = 90,
        [FromQuery] int topLimit = 10,
        [FromQuery] int riskThresholdDays = 60,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var insights = await customerService.GetInsightsAsync(
            new GetCustomerInsightsQuery(days, topLimit, riskThresholdDays),
            cancellationToken);

        return Ok(insights);
    }
}