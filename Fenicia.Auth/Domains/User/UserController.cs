// using Fenicia.Auth.Services.Interfaces;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Fenicia.Auth.Controllers;
//
// [Authorize]
// [ApiController]
// [Route("[controller]")]
// public class UserController(ILogger<UserController> logger, IUserRoleService userRoleService)
//     : ControllerBase
// {
//     [HttpPost("{id}/role")]
//     [Authorize(Roles = "Admin")]
//     public async Task<ActionResult> PostAsync(
//         [FromRoute] Guid id,
//         [FromBody] UserRoleRequest request
//     )
//     {
//         logger.LogInformation("Adding {role} to user {id}", [request.R]);
//
//         var role = await userRoleService.AddRoleToUserAsync(request);
//
//         return Ok();
//     }
// }
