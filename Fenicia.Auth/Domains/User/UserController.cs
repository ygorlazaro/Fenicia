namespace Fenicia.Auth.Domains.User;

using Fenicia.Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Common.Database.Responses;

using Fenicia.Auth.Domains.Module;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IModuleService moduleService;

    public UserController(ILogger<UserController> logger, IModuleService moduleService)
    {
        this.logger = logger;
        this.moduleService = moduleService;
    }

    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ModuleResponse>> GetUserModulesAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(this.User);
        var companyId = ClaimReader.CompanyId(this.User);

        this.logger.LogWarning("Getting log for the user {userID}", userId);

        var response = await this.moduleService.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);
        return this.Ok(response);
    }
}
