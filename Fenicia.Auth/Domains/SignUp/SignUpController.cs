namespace Fenicia.Auth.Domains.SignUp;

using System.Net.Mime;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using User.Data;
using User.Logic;

[AllowAnonymous]
[Route(template: "[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SignUpController(ILogger<SignUpController> logger, IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting user creation process for email: {Email}", request.Email);

            var userResponse = await userService.CreateNewUserAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                logger.LogWarning(message: "User creation failed for email: {Email}. Status: {Status}, Message: {Message}", request.Email, userResponse.Status, userResponse.Message);
                return StatusCode((int)userResponse.Status, userResponse.Message);
            }

            logger.LogInformation(message: "Successfully created new user with email: {Email}", request.Email);
            return Ok(userResponse.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while creating user with email: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, value: "An unexpected error occurred while processing your request.");
        }
    }
}
