using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

public record GetAllProjectStatusQuery(int Page = 1, int PerPage = 10, string? Query = null, string? Sort = null);