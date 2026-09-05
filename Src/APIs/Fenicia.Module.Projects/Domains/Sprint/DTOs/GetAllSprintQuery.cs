namespace Fenicia.Module.Projects.Domains.Sprint.DTOs;

public record GetAllSprintQuery(int Page = 1, int PerPage = 10, Guid? ProjectId = null);
