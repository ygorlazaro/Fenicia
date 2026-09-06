using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Upload.DTOs;

public record UploadFileCommand(IFormFile File);
