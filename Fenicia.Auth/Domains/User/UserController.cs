namespace Fenicia.Auth.Domains.User;

using Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Common.Database.Responses;

using Module;

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
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);

        logger.LogWarning("Getting log for the user {userID}", userId);

        var response = await moduleService.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);
        return Ok(response);
    }
}
