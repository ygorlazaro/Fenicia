using Fenicia.Common.DTOs.Auth.Upload;

namespace Fenicia.Auth.Domains.Upload.Interfaces;

/// <summary>
/// Service interface for managing file upload operations.
/// </summary>
public interface IUploadService
{
    /// <summary>
    /// Uploads a file asynchronously.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="uploadOptions">The upload configuration options.</param>
    /// <param name="contentRootPath">The content root path of the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the upload file response.</returns>
    Task<UploadFileResponse?> UploadFileAsync(IFormFile file, UploadOptions uploadOptions,
        string contentRootPath,
        CancellationToken cancellationToken);
}