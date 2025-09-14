namespace Fenicia.ERP.API.Areas.Auth;

using Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[Route("[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAsync()
    {
        ClaimReader.ValidateRole(User, "Admin");

        return Ok("Sucesso");
    }
}
