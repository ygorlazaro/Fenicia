using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public record DeleteCommentCommand([Required] Guid Id);