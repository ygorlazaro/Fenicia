using Fenicia.Common.DTOs.Auth.Upload;

namespace Fenicia.Auth.Domains.Upload.Interfaces;

public interface IUploadService
{
    Task<UploadFileResponse?> UploadFileAsync(IFormFile file, UploadOptions uploadOptions,
        string contentRootPath,
        CancellationToken cancellationToken);
}