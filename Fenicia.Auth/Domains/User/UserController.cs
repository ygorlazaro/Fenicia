using Fenicia.Auth.Domains.User.GetUserModules;
using Fenicia.Auth.Domains.UserRole.GetUserCompanies;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.User;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet("module")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ModuleResponse))]
    public async Task<ActionResult<List<ModuleResponse>>> GetUserModulesAsync(
        [FromHeader] Headers headers,
        [FromServices] GetUserModuleHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;

        wide.UserId = userId.ToString();
        var query = new GetUserModulesQuery(companyId, userId);

        var response = await handler.Handler(query, ct);

        return Ok(response);
    }

    [HttpGet("company")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCompaniesResponse))]
    public async Task<ActionResult<List<GetUserCompaniesResponse>>> GetUserCompanyAsync(
        [FromServices] GetUserCompaniesHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);

        wide.UserId = userId.ToString();

        var response = await handler.Handle(userId, ct);

        return Ok(response);
    }
}
