using Fenicia.Auth.Requests;
using Fenicia.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewOrderAsync(NewOrderRequest request)
    {
        var userClaim = User.Claims.FirstOrDefault(claimToSearch => string.Equals(claimToSearch.Type, "userId", StringComparison.Ordinal));
        var companyClaim = User.Claims.FirstOrDefault(claimToSearch => string.Equals(claimToSearch.Type, "companyId", StringComparison.Ordinal));
        
        if (userClaim == null || companyClaim == null)
        {
            return Forbid();
        }
        
        var userId = Guid.Parse(userClaim.Value);
        var companyId = Guid.Parse(companyClaim.Value);
        
        var order = await orderService.CreateNewOrderAsync(userId, companyId, request);
        
        return Ok(order);
    }
}