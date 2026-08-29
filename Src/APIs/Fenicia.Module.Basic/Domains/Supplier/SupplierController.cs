using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Supplier;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class SupplierController(SupplierService supplierService) : ControllerBase
{
    /// <summary>
    /// Obtém uma lista paginada de fornecedores.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de fornecedores</returns>
    /// <response code="200">Lista de fornecedores retornada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllSupplierResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllSupplierResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var suppliers = await supplierService.GetAllAsync(new GetAllSupplierQuery(page, perPage), ct);

            return Ok(suppliers);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém um fornecedor pelo ID.
    /// </summary>
    /// <param name="id">ID do fornecedor</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do fornecedor</returns>
    /// <response code="200">Fornecedor encontrado</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Fornecedor não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetSupplierByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSupplierByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var supplier = await supplierService.GetByIdAsync(new GetSupplierByIdQuery(id), ct);

            return supplier is null ? NotFound() : Ok(supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Cria um novo fornecedor.
    /// </summary>
    /// <param name="command">Dados do fornecedor a ser criado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Fornecedor criado</returns>
    /// <response code="201">Fornecedor criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddSupplierResponse>> PostAsync([FromBody] AddSupplierCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = ClaimReader.UserId(User);
            var supplier = await supplierService.AddAsync(command, companyId, ct);

            return new CreatedResult(string.Empty, supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um fornecedor existente.
    /// </summary>
    /// <param name="command">Dados atualizados do fornecedor</param>
    /// <param name="id">ID do fornecedor</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Fornecedor atualizado</returns>
    /// <response code="200">Fornecedor atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Fornecedor não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateSupplierResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateSupplierResponse>> PatchAsync([FromBody] UpdateSupplierCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = ClaimReader.UserId(User);
            var supplier = await supplierService.UpdateAsync(command with { Id = id }, companyId, ct);

            return supplier is null ? NotFound() : Ok(supplier);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Remove um fornecedor (soft delete).
    /// </summary>
    /// <param name="id">ID do fornecedor</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <response code="204">Fornecedor removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var companyId = ClaimReader.UserId(User);
            await supplierService.DeleteAsync(new DeleteSupplierCommand(id), companyId, ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém métricas de desempenho dos fornecedores.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Período em dias para análise</param>
    /// <param name="topLimit">Limite de registros no top</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Métricas de desempenho dos fornecedores</returns>
    /// <response code="200">Desempenho retornado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SupplierPerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SupplierPerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await supplierService.GetPerformanceAsync(new GetSupplierPerformanceQuery(days, topLimit), ct);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
