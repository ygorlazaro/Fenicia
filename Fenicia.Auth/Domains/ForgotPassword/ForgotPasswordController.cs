namespace Fenicia.Auth.Domains.ForgotPassword;

using System.Net.Mime;

using Common.Database.Requests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[AllowAnonymous]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ForgotPasswordController : ControllerBase
{
    private readonly ILogger<ForgotPasswordController> _logger;
    private readonly IForgotPasswordService _forgotPasswordService;

    public ForgotPasswordController(ILogger<ForgotPasswordController> logger, IForgotPasswordService forgotPasswordService)
    {
        _logger = logger;
        _forgotPasswordService = forgotPasswordService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting password recovery process for user request");

            var response = await _forgotPasswordService.SaveForgotPasswordAsync(request, cancellationToken);

            _logger.LogInformation("Password recovery process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password recovery process");
            throw;
        }
    }

    [HttpPost("reset")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] ForgotPasswordRequestReset request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting password reset process");

            var response = await _forgotPasswordService.ResetPasswordAsync(request, cancellationToken);

            _logger.LogInformation("Password reset process completed with status: {Status}", response.Status);

            return response.Data switch
            {
                null => StatusCode((int)response.Status, response.Message),
                _ => Ok(response)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password reset process");
            throw;
        }
    }
}
