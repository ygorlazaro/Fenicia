using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record GetUserForRefreshQuery([Required] Guid UserId);