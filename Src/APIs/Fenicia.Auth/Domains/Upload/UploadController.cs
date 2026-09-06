using System.IO;
using System.Net.Mime;
using Fenicia.Auth.Domains.Upload.DTOs;
using Fenicia.Common.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Fenicia.Auth.Domains.Upload;

[Authorize]
[ApiController]
[Route("upload")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class UploadController(IOptions<UploadOptions> options, IWebHostEnvironment env) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UploadFileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<UploadFileResponse>> PostAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Arquivo inválido." });
        }

        var uploadOptions = options.Value;
        var maxSize = uploadOptions.MaxFileSizeBytes > 0 ? uploadOptions.MaxFileSizeBytes : 5 * 1024 * 1024;
        if (file.Length > maxSize)
        {
            return BadRequest(new { message = $"Arquivo excede o tamanho máximo de {maxSize / 1024 / 1024}MB." });
        }

        var uploadRoot = uploadOptions.Directory;
        if (Path.IsPathRooted(uploadRoot))
        {
            uploadRoot = uploadRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        else
        {
            uploadRoot = Path.Combine(env.ContentRootPath, uploadRoot.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        Directory.CreateDirectory(uploadRoot);

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadRoot, storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, useAsync: true);
        await file.CopyToAsync(stream, cancellationToken);

        var url = $"/upload/{storedFileName}";
        var response = new UploadFileResponse(
            Guid.NewGuid(),
            Path.GetFileName(file.FileName),
            storedFileName,
            file.ContentType,
            file.Length,
            url);

        return Ok(response);
    }

    #pragma warning disable CA3003
    [HttpGet("{fileName}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAsync(string fileName, CancellationToken cancellationToken)
    {
        var safeFileName = Path.GetFileName(fileName);
        if (string.IsNullOrEmpty(safeFileName))
        {
            return BadRequest();
        }

        var uploadOptions = options.Value;
        var uploadRoot = uploadOptions.Directory;
        if (Path.IsPathRooted(uploadRoot))
        {
            uploadRoot = uploadRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }
        else
        {
            uploadRoot = Path.Combine(env.ContentRootPath, uploadRoot.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        }

        var fullPath = Path.Combine(uploadRoot, safeFileName);
        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound();
        }

        var contentType = fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ? "image/png"
            : fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg"
            : fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ? "image/gif"
            : fileName.EndsWith(".webp", StringComparison.OrdinalIgnoreCase) ? "image/webp"
            : "application/octet-stream";

        var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read, 8192, useAsync: true);
        return File(stream, contentType, enableRangeProcessing: true);
    }
    #pragma warning restore CA3003
}
