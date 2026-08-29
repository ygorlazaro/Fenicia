namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record ProjectTaskResponse(Guid Id, string Title, string? Description, string Priority, string Type, int? EstimatePoints, DateTime? DueDate);