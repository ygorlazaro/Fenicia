using Fenicia.Auth.Domains.Module.Logic;
using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

using Common.Database.Responses;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IModuleService _moduleService;

    public UserController(ILogger<UserController> logger, IModuleService moduleService)
    {
        _logger = logger;
        _moduleService = moduleService;
    }

    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ModuleResponse>> GetUserModulesAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);

        _logger.LogWarning("Getting log for the user {userId}", userId);

        var response = await _moduleService.GetUserModulesAsync(userId, companyId, cancellationToken);
        return Ok(response);
    }
}
