namespace Fenicia.Common.DTOs.Auth.Upload;

public class UploadOptions
{
    public string Directory { get; set; } = "/upload";

    public long MaxFileSizeBytes { get; set; } = 5 * 1024 * 1024;
}
