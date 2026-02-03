using brevo_csharp.Client;

using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Api;
using Fenicia.Common.API;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController(IModuleService moduleService, IUserRoleService userRoleService) : ControllerBase
{
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<ApiResponse<List<ModuleResponse>>>> GetUserModulesAsync([FromHeader] Headers headers, WideEventContext wide, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = headers.CompanyId;

        wide.Operation = "Get User Modules";
        wide.UserId = userId.ToString();

        var response = await moduleService.GetModuleAndSubmoduleAsync(userId, companyId, cancellationToken);

        return Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponse))]
    public async Task<ActionResult<ApiResponse<CompanyResponse>>> GetUserCompanyAsync(WideEventContext wide, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);

        wide.Operation = "Get User Company";
        wide.UserId = userId.ToString();

        var response = await userRoleService.GetUserCompaniesAsync(userId, cancellationToken);

        return Ok(response);
    }
}
