using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Dashboard;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    /// <summary>
    ///     Obtém o dashboard financeiro com KPIs, receita vs custo, margem de lucro, contas a receber e vendas diárias.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="days">Quantidade de dias para análise (padrão: 90)</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dashboard financeiro completo</returns>
    /// <response code="200">Dashboard financeiro retornado com sucesso</response>
    /// <response code="401">Usuário não autorizado</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar o dashboard</exception>
    [HttpGet("financial")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FinancialDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FinancialDashboardResponse>> GetFinancialDashboardAsync(
        WideEventContext wide,
        [FromQuery] int days = 90,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await dashboardService.GetFinancialDashboardAsync(
                new GetFinancialDashboardQuery(days),
                cancellationToken);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}