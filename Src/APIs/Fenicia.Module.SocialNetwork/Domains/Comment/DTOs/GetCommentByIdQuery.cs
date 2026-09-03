using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Comment.DTOs;

public record GetCommentByIdQuery([Required] Guid Id);