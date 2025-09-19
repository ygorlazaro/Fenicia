namespace Fenicia.Auth.Domains.Register;

using System.Net.Mime;

using Common.Database.Requests;
using Common.Database.Responses;

using User;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController : ControllerBase
{
    private readonly ILogger<RegisterController> _logger;
    private readonly IUserService _userService;

    public RegisterController(ILogger<RegisterController> logger, IUserService userService)
    {
        this._logger = logger;
        this._userService = userService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting user creation process for email: {Email}", request.Email);

            var userResponse = await _userService.CreateNewUserAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                _logger.LogWarning("User creation failed for email: {Email}. Status: {Status}, Message: {Message}", request.Email, userResponse.Status, userResponse.Message);
                return StatusCode((int)userResponse.Status, userResponse.Message);
            }

            _logger.LogInformation("Successfully created new user with email: {Email}", request.Email);
            return Ok(userResponse.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user with email: {Email}", request.Email);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while processing your request.");
        }
    }
}
