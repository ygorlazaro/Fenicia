namespace Fenicia.Common.DTOs.Project.Sprint;

public class GetAllSprintQuery()
{
    public GetAllSprintQuery(int page = 1, int perPage = 10, Guid? projectId = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        ProjectId = projectId;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public Guid? ProjectId { get; set; }
}
