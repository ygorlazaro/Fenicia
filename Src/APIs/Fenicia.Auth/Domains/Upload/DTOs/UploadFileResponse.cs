using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Upload.DTOs;

public record UploadFileResponse(
    [Required] Guid Id,
    [Required] string OriginalFileName,
    [Required] string StoredFileName,
    [Required] string ContentType,
    long SizeBytes,
    string? Url);
