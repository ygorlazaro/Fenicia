using Fenicia.Auth.Requests;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common.Api;
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
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);

        var order = await orderService.CreateNewOrderAsync(userId, companyId, request);

        return Ok(order);
    }
}