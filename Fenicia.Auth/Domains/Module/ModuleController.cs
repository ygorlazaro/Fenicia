using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(ILogger<ModuleController> logger, IModuleService moduleService) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving modules with pagination: Page {Page}, Items per page {PerPage}", query.Page, query.PerPage);

            var modules = await moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
            var total = await moduleService.CountAsync(cancellationToken);

            if (modules.Data is null)
            {
                logger.LogWarning("No modules data found. Status: {Status}, Message: {Message}", modules.Status, modules.Message);

                return this.StatusCode((int)modules.Status, modules.Message);
            }

            var pagination = new Pagination<List<ModuleResponse>>(modules.Data, total.Data, query.Page, query.PerPage);

            logger.LogInformation("Successfully retrieved {Count} modules", modules.Data.Count);

            return this.Ok(pagination);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving modules");

            return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }
}
