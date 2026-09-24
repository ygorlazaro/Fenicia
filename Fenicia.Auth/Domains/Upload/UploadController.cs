using System.Net.Mime;
using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.DTOs.Auth.Upload;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fenicia.Auth.Domains.Upload;

[Authorize]
[ApiController]
[Route("upload")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UploadController(IUploadService service, IOptions<UploadOptions> options, IWebHostEnvironment env) : ControllerBase
{
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
