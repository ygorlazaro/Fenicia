using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.Auth;

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
    public async Task<ActionResult<List<ModuleResponse>>> GetUserModulesAsync([FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = headers.CompanyId;

        wide.UserId = userId.ToString();

        var response = await moduleService.GetModuleAndSubmoduleAsync(userId, companyId, ct);

        return Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CompanyResponse))]
    public async Task<ActionResult<CompanyResponse>> GetUserCompanyAsync(WideEventContext wide, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(User);

        wide.UserId = userId.ToString();

        var response = await userRoleService.GetUserCompaniesAsync(userId, ct);

        return Ok(response);
    }
}
