using System.Net.Mime;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(ILogger<RegisterController> logger, IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting user creation process for email: {Email}", request.Email);

            var userResponse = await userService.CreateNewUserAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                logger.LogWarning("User creation failed for email: {Email}. Status: {Status}, Message: {Message}", request.Email, userResponse.Status, userResponse.Message);
                return this.StatusCode((int)userResponse.Status, userResponse.Message);
            }

            logger.LogInformation("Successfully created new user with email: {Email}", request.Email);

            return this.Ok(userResponse.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while creating user with email: {Email}", request.Email);

            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing your request.");
        }
    }
}
