using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment.DTOs;

public record DeleteAttachmentCommand(
    [Required] Guid Id);
