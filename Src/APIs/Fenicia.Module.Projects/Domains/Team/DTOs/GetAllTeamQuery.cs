using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.Team.DTOs;

public record GetAllTeamQuery(int Page = 1, int PerPage = 10);
