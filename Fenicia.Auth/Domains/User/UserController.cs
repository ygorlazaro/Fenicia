using brevo_csharp.Client;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.API;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController(ILogger<UserController> logger, IModuleService moduleService, IUserRoleService userRoleService) : ControllerBase
{
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ApiResponse<List<ModuleResponse>>>> GetUserModulesAsync([FromHeader] Headers headers, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;

        logger.LogWarning("Getting log for the user {userID}", userId);

        var response = await moduleService.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);

        return this.Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponse))]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetUserCompanyAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(this.User);

        logger.LogWarning("Getting companies for the user {userID}", userId);

        var response = await userRoleService.GetUserCompaniesAsync(userId, cancellationToken);

        return this.Ok(response);
    }
}
