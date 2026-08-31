using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record GetProjectStatusByIdQuery([Required] Guid Id);