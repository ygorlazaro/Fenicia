namespace Fenicia.ERP.API.Areas.Auth;

using Common.API;

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
        ClaimReader.ValidateRole(this.User, "Admin");

        return this.Ok("Sucesso");
    }
}
