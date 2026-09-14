using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Attachment;

public record DeleteAttachmentCommand([Required] Guid Id);