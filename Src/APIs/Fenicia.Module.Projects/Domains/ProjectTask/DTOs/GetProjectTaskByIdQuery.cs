using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record GetProjectTaskByIdQuery([Required] Guid Id);