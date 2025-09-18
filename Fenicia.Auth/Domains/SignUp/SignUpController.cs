namespace Fenicia.Auth.Domains.SignUp;

using System.Net.Mime;

using Common.Database.Requests;
using Common.Database.Responses;

using Fenicia.Auth.Domains.User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SignUpController : ControllerBase
{
    private readonly ILogger<SignUpController> logger;
    private readonly IUserService userService;

    public SignUpController(ILogger<SignUpController> logger, IUserService userService)
    {
        this.logger = logger;
        this.userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Starting user creation process for email: {Email}", request.Email);

            var userResponse = await this.userService.CreateNewUserAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                this.logger.LogWarning("User creation failed for email: {Email}. Status: {Status}, Message: {Message}", request.Email, userResponse.Status, userResponse.Message);
                return this.StatusCode((int)userResponse.Status, userResponse.Message);
            }

            this.logger.LogInformation("Successfully created new user with email: {Email}", request.Email);
            return this.Ok(userResponse.Data);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error occurred while creating user with email: {Email}", request.Email);
            return this.StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing your request.");
        }
    }
}
