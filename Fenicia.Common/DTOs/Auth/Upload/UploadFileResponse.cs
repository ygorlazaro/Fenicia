using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Upload;

/// <summary>
/// Response DTO containing uploaded file details.
/// </summary>
public record UploadFileResponse(
    /// <summary>
    /// The unique identifier of the uploaded file.
    /// </summary>
    [Required] Guid Id,
    
    /// <summary>
    /// The original file name.
    /// </summary>
    [Required] string OriginalFileName,
    
    /// <summary>
    /// The stored file name on the server.
    /// </summary>
    [Required] string StoredFileName,
    
    /// <summary>
    /// The content type (MIME type) of the file.
    /// </summary>
    [Required] string ContentType,
    
    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    long SizeBytes,
    
    /// <summary>
    /// The URL to access the uploaded file (optional).
    /// </summary>
    string? Url);
