using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserController: ControllerBase
{
    [HttpGet("god")]
    [Authorize(Roles = "God")]
    public IActionResult GetGod()
    {
        return Ok("God");
    }
    
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAdmin()
    {
        return Ok("Admin");
    }

    [HttpGet("user")]
    [Authorize(Roles = "User")]
    public IActionResult GetUser()
    {
        return Ok("User");
    }


}