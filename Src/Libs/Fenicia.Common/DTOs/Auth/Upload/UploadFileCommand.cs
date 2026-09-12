using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.DTOs.Auth.Upload;

public record UploadFileCommand(IFormFile File);
