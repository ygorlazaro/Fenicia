using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record DeleteProjectTaskCommand([Required] Guid Id);