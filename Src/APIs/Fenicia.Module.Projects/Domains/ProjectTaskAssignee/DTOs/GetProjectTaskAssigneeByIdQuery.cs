using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record GetProjectTaskAssigneeByIdQuery([Required] Guid Id);