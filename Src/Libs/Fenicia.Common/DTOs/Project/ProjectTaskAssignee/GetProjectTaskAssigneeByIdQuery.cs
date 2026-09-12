using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTaskAssignee;

public record GetProjectTaskAssigneeByIdQuery([Required] Guid Id);