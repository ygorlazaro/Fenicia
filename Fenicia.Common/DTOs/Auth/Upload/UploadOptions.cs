namespace Fenicia.Common.DTOs.Auth.Upload;

/// <summary>
/// Configuration options for file uploads.
/// </summary>
public class UploadOptions
{
    /// <summary>
    /// Gets or sets the upload directory path.
    /// </summary>
    public string Directory { get; set; } = "/upload";

    /// <summary>
    /// Gets or sets the maximum file size in bytes (default: 5 MB).
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
}
