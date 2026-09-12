using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Upload;

public record UploadFileResponse(
    [Required] Guid Id,
    [Required] string OriginalFileName,
    [Required] string StoredFileName,
    [Required] string ContentType,
    long SizeBytes,
    string? Url);
