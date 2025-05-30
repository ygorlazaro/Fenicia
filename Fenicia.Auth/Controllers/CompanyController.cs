using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CompanyController(ICompanyService companyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByLoggedUser()
    {
        var userId = ClaimReader.UserId(User);
        var companies = await companyService.GetByUserIdAsync(userId);

        return Ok(companies);
    }
}