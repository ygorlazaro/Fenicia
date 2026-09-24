using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Upload;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Upload;

public class UploadService(IUploadRepository repository) : IUploadService
{
    public async Task<UploadFileResponse?> UploadFileAsync(IFormFile file, UploadOptions uploadOptions, string
            contentRootPath, CancellationToken cancellationToken)
    {
        var maxSize = uploadOptions.MaxFileSizeBytes > 0 ? uploadOptions.MaxFileSizeBytes : 5 * 1024 * 1024;

        if (file.Length > maxSize)
        {
            throw new BadRequestException($"Arquivo excede o tamanho máximo de {maxSize / 1024 / 1024}MB.");
        }

        var uploadRoot = uploadOptions.Directory;

        uploadRoot = Path.IsPathRooted(uploadRoot) switch
        {
            true => uploadRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            false => Path.Combine(contentRootPath,
                uploadRoot.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
        };

        Directory.CreateDirectory(uploadRoot);

        var extension = Path.GetExtension(file.FileName);
        var storedFileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(uploadRoot, storedFileName);

        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await file.CopyToAsync(stream, cancellationToken);

        var url = $"/upload/{storedFileName}";

        var upload = new UploadModel
        {
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            Url = url
        };

        await repository.InsertAsync(upload, cancellationToken);

        var response = new UploadFileResponse(
            upload.Id,
            upload.OriginalFileName,
            upload.StoredFileName,
            upload.ContentType,
            upload.SizeBytes,
            upload.Url);
        return response;
    }
}
