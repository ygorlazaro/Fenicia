using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

public record DeleteProjectTaskAssigneeCommand([Required] Guid Id);