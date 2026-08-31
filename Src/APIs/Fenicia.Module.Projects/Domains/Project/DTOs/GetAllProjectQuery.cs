using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Project.DTOs;

public record GetAllProjectQuery(int Page = 1, int PerPage = 10);