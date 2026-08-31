using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;

public record GetProjectSubtaskByIdQuery([Required] Guid Id);