using Fenicia.Auth.Domains.Module.Data;
using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController(ILogger<UserController> logger, IModuleService moduleService) : ControllerBase
{

    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ModuleResponse>> GetUserModulesAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);

        logger.LogWarning("Getting log for the user {userId}", userId);

        var response = await moduleService.GetUserModulesAsync(userId, companyId, cancellationToken);
        return Ok(response);
    }
}
