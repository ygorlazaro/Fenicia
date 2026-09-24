using System.Net.Mime;
using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.DTOs.Auth.Upload;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fenicia.Auth.Domains.Upload;

/// <summary>
/// Controller for managing file upload operations.
/// </summary>
[Authorize]
[ApiController]
[Route("upload")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UploadController(IUploadService service, IOptions<UploadOptions> options, IWebHostEnvironment env) : ControllerBase
{
    /// <summary>
    /// Uploads a file.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The upload file response.</returns>
    /// <response code="200">File uploaded successfully</response>
    /// <response code="400">Invalid file or file exceeds maximum size</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(UploadFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadFileResponse>> PostAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        try
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest(new { message = "Arquivo inválido." });
            }

            var uploadOptions = options.Value;
            var contentRootPath = env.ContentRootPath;

            var response = await service.UploadFileAsync(file, uploadOptions, contentRootPath, cancellationToken);
            return Ok(response);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
