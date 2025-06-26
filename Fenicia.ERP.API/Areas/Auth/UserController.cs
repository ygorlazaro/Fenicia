namespace Fenicia.ERP.API.Areas.Auth;

using Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route(template: "[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAsync()
    {
        ClaimReader.ValidateRole(User, roleToSearch: "Admin");

        return Ok(value: "Sucesso");
    }
}
