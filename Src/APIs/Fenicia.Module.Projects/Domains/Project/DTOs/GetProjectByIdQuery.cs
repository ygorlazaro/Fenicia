using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record GetProjectByIdQuery([Required] Guid Id);