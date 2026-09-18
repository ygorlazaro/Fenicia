namespace Fenicia.Common.DTOs.Project.Team;

public class GetAllTeamQuery()
{
    public GetAllTeamQuery(int page = 1, int perPage = 10)
        : this()
    {
        Page = page;
        PerPage = perPage;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;
}
