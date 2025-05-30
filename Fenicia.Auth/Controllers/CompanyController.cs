using Fenicia.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class CompanyController(ICompanyService companyService): ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByLoggedUser()
    {
        var claim = User.Claims.FirstOrDefault(claimToSearch => string.Equals(claimToSearch.Type, "userId", StringComparison.Ordinal));

        if (claim == null)
        {
            return Forbid();
        }
        
        var userId = Guid.Parse(claim.Value);
        
        var companies = await companyService.GetByUserIdAsync(userId);
        
        return Ok(companies);
    }
}