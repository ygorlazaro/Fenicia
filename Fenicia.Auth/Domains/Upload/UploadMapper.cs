using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Upload;

namespace Fenicia.Auth.Domains.Upload;

/// <summary>
/// Mapper class for mapping between UploadModel and UploadFileResponse.
/// </summary>
public static class UploadMapper
{
    /// <summary>
    /// Maps an IFormFile and stored file name to an UploadModel.
    /// </summary>
    /// <param name="file">The uploaded file.</param>
    /// <param name="storedFileName">The generated stored file name.</param>
    /// <param name="url">The access URL for the file.</param>
    /// <returns>The created upload model.</returns>
    public static UploadModel MapToUploadModel(IFormFile file, string storedFileName, string url)
    {
        return new UploadModel
        {
            OriginalFileName = Path.GetFileName(file.FileName),
            StoredFileName = storedFileName,
            ContentType = file.ContentType,
            SizeBytes = file.Length,
            Url = url
        };
    }

    /// <summary>
    /// Maps an UploadModel to an UploadFileResponse.
    /// </summary>
    /// <param name="upload">The upload model.</param>
    /// <returns>The upload file response.</returns>
    public static UploadFileResponse MapToUploadFileResponse(UploadModel upload)
    {
        return new UploadFileResponse(
            upload.Id,
            upload.OriginalFileName,
            upload.StoredFileName,
            upload.ContentType,
            upload.SizeBytes,
            upload.Url);
    }
}