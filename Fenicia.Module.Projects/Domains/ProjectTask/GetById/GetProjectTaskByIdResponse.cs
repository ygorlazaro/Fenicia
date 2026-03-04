namespace Fenicia.Module.Projects.Domains.ProjectTask.GetById;

public record GetProjectTaskByIdResponse(
    Guid Id,
    Guid ProjectId,
    Guid StatusId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    int Order,
    int? EstimatePoints,
    DateTime? DueDate,
    Guid CreatedBy,
    Guid CompanyId,
    List<ProjectAttachmentResponse> Attachments,
    List<ProjectCommentResponse> Comments,
    List<ProjectSubtaskResponse> Subtasks,
    List<ProjectTaskAssigneeResponse> Assignees);