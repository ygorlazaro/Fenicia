namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class GetAllProjectTaskQuery()
{
    public GetAllProjectTaskQuery(
        int page = 1,
        int perPage = 10,
        string? query = null,
        string? sort = null,
        Guid? statusId = null,
        Guid? createdBy = null,
        Guid? assigneeId = null,
        DateTime? dueFrom = null,
        DateTime? dueTo = null,
        string? type = null,
        string? priority = null,
        Guid? sprintId = null,
        bool? withoutSprint = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        Query = query;
        Sort = sort;
        StatusId = statusId;
        CreatedBy = createdBy;
        AssigneeId = assigneeId;
        DueFrom = dueFrom;
        DueTo = dueTo;
        Type = type;
        Priority = priority;
        SprintId = sprintId;
        WithoutSprint = withoutSprint;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    public string? Query { get; set; }

    public string? Sort { get; set; }

    public Guid? StatusId { get; set; }

    public Guid? CreatedBy { get; set; }

    public Guid? AssigneeId { get; set; }

    public DateTime? DueFrom { get; set; }

    public DateTime? DueTo { get; set; }

    public string? Type { get; set; }

    public string? Priority { get; set; }

    public Guid? SprintId { get; set; }

    public bool? WithoutSprint { get; set; }
}
