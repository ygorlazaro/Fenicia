using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;

using Fenicia.Module.Basic.Domains.Employee.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Employee;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class EmployeeController(EmployeeService employeeService) : ControllerBase
{
    /// <summary>
    /// Obtém a lista de funcionários.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="perPage">Quantidade de registros por página (padrão: 10)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de funcionários</returns>
    /// <response code="200">Lista de funcionários retornada com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os funcionários</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllEmployeeResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllEmployeeResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employees = await employeeService.GetAllAsync(new GetAllEmployeeQuery(page, perPage), ct);

            return Ok(employees);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém um funcionário pelo ID.
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados do funcionário</returns>
    /// <response code="200">Funcionário encontrado</response>
    /// <response code="404">Funcionário não encontrado</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o funcionário</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetEmployeeByIdResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetEmployeeByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.GetByIdAsync(new GetEmployeeByIdQuery(id), ct);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Cria um novo funcionário.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="command">Dados do funcionário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Funcionário criado</returns>
    /// <response code="201">Funcionário criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a criar funcionário</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddEmployeeResponse>> PostAsync([FromBody] AddEmployeeCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.AddAsync(command, ClaimReader.UserId(User), ct);

            return new CreatedResult(string.Empty, employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza um funcionário existente.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="command">Dados atualizados do funcionário</param>
    /// <param name="id">ID do funcionário</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Funcionário atualizado</returns>
    /// <response code="200">Funcionário atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <response code="404">Funcionário não encontrado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a atualizar funcionário</exception>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateEmployeeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateEmployeeResponse>> PatchAsync([FromBody] UpdateEmployeeCommand command, [FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var employee = await employeeService.UpdateAsync(command with { Id = id }, ClaimReader.UserId(User), ct);

            return employee is null ? NotFound() : Ok(employee);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Remove um funcionário.
    /// </summary>
    /// <param name="id">ID do funcionário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Funcionário removido com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a remover funcionário</exception>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await employeeService.DeleteAsync(new DeleteEmployeeCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Obtém o desempenho dos funcionários.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Quantidade de dias para análise (padrão: 90)</param>
    /// <param name="topLimit">Limite de top performers (padrão: 10)</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Dados de desempenho dos funcionários</returns>
    /// <response code="200">Desempenho retornado com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o desempenho</exception>
    [HttpGet("performance")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePerformanceResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<EmployeePerformanceResponse>> GetPerformanceAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var performance = await employeeService.GetPerformanceAsync(new GetEmployeePerformanceQuery(days, topLimit), ct);

            return Ok(performance);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
