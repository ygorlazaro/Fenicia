using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Upload;
using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Upload;

/// <summary>
/// Service implementation for managing file upload operations.
/// </summary>
/// <param name="repository">The upload repository.</param>
public class UploadService(IUploadRepository repository) : IUploadService
{
    /// <summary>
    /// Uploads a file asynchronously.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="uploadOptions">The upload configuration options.</param>
    /// <param name="contentRootPath">The content root path of the application.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the upload file response.</returns>
    /// <exception cref="BadRequestException">Thrown when file exceeds maximum allowed size.</exception>
    public async Task<UploadFileResponse?> UploadFileAsync(IFormFile file, UploadOptions uploadOptions, string
            contentRootPath, CancellationToken cancellationToken)
    {
        ValidateFileSize(file, uploadOptions);

        var uploadRoot = ResolveUploadRoot(uploadOptions, contentRootPath);
        EnsureDirectoryExists(uploadRoot);

        var storedFileName = GenerateStoredFileName(file);
        var fullPath = Path.Combine(uploadRoot, storedFileName);

        await SaveFileAsync(file, fullPath, cancellationToken);

        var url = $"/upload/{storedFileName}";
        var upload = UploadMapper.MapToUploadModel(file, storedFileName, url);

        await repository.InsertAsync(upload, cancellationToken);

        return UploadMapper.MapToUploadFileResponse(upload);
    }

    /// <summary>
    /// Validates that the file size does not exceed the maximum allowed size.
    /// </summary>
    /// <param name="file">The file to validate.</param>
    /// <param name="uploadOptions">The upload configuration options.</param>
    /// <exception cref="BadRequestException">Thrown when file exceeds maximum allowed size.</exception>
    private static void ValidateFileSize(IFormFile file, UploadOptions uploadOptions)
    {
        var maxSize = uploadOptions.MaxFileSizeBytes > 0 ? uploadOptions.MaxFileSizeBytes : 5 * 1024 * 1024;

        if (file.Length > maxSize)
        {
            throw new BadRequestException($"Arquivo excede o tamanho máximo de {maxSize / 1024 / 1024}MB.");
        }
    }

    /// <summary>
    /// Resolves the absolute upload root directory path.
    /// </summary>
    /// <param name="uploadOptions">The upload configuration options.</param>
    /// <param name="contentRootPath">The content root path of the application.</param>
    /// <returns>The resolved absolute upload root path.</returns>
    private static string ResolveUploadRoot(UploadOptions uploadOptions, string contentRootPath)
    {
        var uploadRoot = uploadOptions.Directory;

        return Path.IsPathRooted(uploadRoot) switch
        {
            true => uploadRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
            false => Path.Combine(contentRootPath,
                uploadRoot.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
        };
    }

    /// <summary>
    /// Ensures the upload directory exists, creating it if necessary.
    /// </summary>
    /// <param name="uploadRoot">The upload root directory path.</param>
    private static void EnsureDirectoryExists(string uploadRoot)
    {
        Directory.CreateDirectory(uploadRoot);
    }

    /// <summary>
    /// Generates a unique stored file name preserving the original extension.
    /// </summary>
    /// <param name="file">The file to generate a name for.</param>
    /// <returns>A unique stored file name.</returns>
    private static string GenerateStoredFileName(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        return $"{Guid.NewGuid()}{extension}";
    }

    /// <summary>
    /// Saves the file to the specified path asynchronously.
    /// </summary>
    /// <param name="file">The file to save.</param>
    /// <param name="fullPath">The full path where the file will be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous file save operation.</returns>
    private static async Task SaveFileAsync(IFormFile file, string fullPath, CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);
        await file.CopyToAsync(stream, cancellationToken);
    }
}
