namespace Fenicia.Auth.Domains.Module;

using System.Net.Mime;

using Common;
using Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController : ControllerBase
{
    private readonly ILogger<ModuleController> logger;
    private readonly IModuleService moduleService;

    public ModuleController(ILogger<ModuleController> logger, IModuleService moduleService)
    {
        this.logger = logger;
        this.moduleService = moduleService;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<ModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<ModuleResponse>>>> GetAllModulesAsync([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Retrieving modules with pagination: Page {Page}, Items per page {PerPage}", query.Page, query.PerPage);

            var modules = await this.moduleService.GetAllOrderedAsync(cancellationToken, query.Page, query.PerPage);
            var total = await this.moduleService.CountAsync(cancellationToken);

            if (modules.Data is null)
            {
                this.logger.LogWarning("No modules data found. Status: {Status}, Message: {Message}", modules.Status, modules.Message);
                return this.StatusCode((int)modules.Status, modules.Message);
            }

            var pagination = new Pagination<List<ModuleResponse>>(modules.Data, total.Data, query.Page, query.PerPage);

            this.logger.LogInformation("Successfully retrieved {Count} modules", modules.Data.Count);
            return this.Ok(pagination);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error occurred while retrieving modules");
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request");
        }
    }
}
