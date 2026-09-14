using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTaskAssignee;

public record DeleteProjectTaskAssigneeCommand([Required] Guid Id);