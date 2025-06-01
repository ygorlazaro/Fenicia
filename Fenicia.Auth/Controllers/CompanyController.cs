using Fenicia.Auth.Contexts.Models;
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

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchAsync(CompanyModel request, Guid id)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);
        
        if (id != companyId)
        {
            return Unauthorized();
        }

        var response = await companyService.PatchAsync(id, userId, request);

        return response is null ? BadRequest() : Ok(response);
    }
}