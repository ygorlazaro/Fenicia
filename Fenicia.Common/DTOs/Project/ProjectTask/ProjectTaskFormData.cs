using Fenicia.Common.DTOs.Project.ProjectComment;
using Fenicia.Common.DTOs.Project.ProjectSubtask;
using Fenicia.Common.DTOs.Project.ProjectTask;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class ProjectTaskFormData
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public Guid StatusId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Priority { get; set; } = "Medium";

    public string Type { get; set; } = "Task";

    public int Order { get; set; }

    public int? EstimatePoints { get; set; }

    public DateTime? DueDate { get; set; }

    public Guid CreatedBy { get; set; }

    public Guid? SprintId { get; set; }

    public List<ProjectTaskAssigneeResponse> Assignees { get; set; } = [];

    public List<GetAllProjectSubtaskResponse> Subtasks { get; set; } = [];

    public List<GetAllProjectCommentResponse> Comments { get; set; } = [];
}
