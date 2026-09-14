namespace Fenicia.Common.DTOs.Project.Sprint;

public record GetAllSprintQuery(int Page = 1, int PerPage = 10, Guid? ProjectId = null);
