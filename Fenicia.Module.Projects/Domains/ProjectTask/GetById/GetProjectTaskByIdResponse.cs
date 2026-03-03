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

public record ProjectAttachmentResponse(
    Guid Id,
    string FileName,
    string ContentType,
    long Size);

public record ProjectCommentResponse(
    Guid Id,
    string Content,
    Guid AuthorId);

public record ProjectSubtaskResponse(
    Guid Id,
    string Title,
    bool IsCompleted,
    int Order,
    DateTime? DueDate);

public record ProjectTaskAssigneeResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    string UserEmail);
