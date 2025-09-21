namespace Fenicia.Auth.Domains.User;

using Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Common.Database.Responses;

using Module;
using brevo_csharp.Client;
using Fenicia.Auth.Domains.UserRole;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IModuleService moduleService;
    private readonly IUserRoleService userRoleService;

    public UserController(ILogger<UserController> logger, IModuleService moduleService, IUserRoleService userRoleService)
    {
        this.logger = logger;
        this.moduleService = moduleService;
        this.userRoleService = userRoleService;
    }

    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ApiResponse<List<ModuleResponse>>>> GetUserModulesAsync(CancellationToken cancellationToken, [FromHeader] Headers headers)
    {
        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;

        this.logger.LogWarning("Getting log for the user {userID}", userId);

        var response = await this.moduleService.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);
        return this.Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponse))]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetUserCompanyAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(this.User);

        this.logger.LogWarning("Getting companies for the user {userID}", userId);

        var response = await this.userRoleService.GetUserCompaniesAsync(userId, cancellationToken);

        return this.Ok(response);
    }
}
