using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record GetUserForRefreshQuery([Required] Guid UserId);