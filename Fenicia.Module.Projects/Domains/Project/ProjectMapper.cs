using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Project;

namespace Fenicia.Module.Projects.Domains.Project;

/// <summary>
/// Mapper class for mapping between ProjectModel and Project DTOs.
/// </summary>
public static class ProjectMapper
{
    /// <summary>
    /// Maps a ProjectModel to a ProjectResponse.
    /// </summary>
    /// <param name="project">The project model.</param>
    /// <returns>The project response.</returns>
    public static ProjectResponse MapToProjectResponse(ProjectModel project)
    {
        return new ProjectResponse(
            project.Id,
            project.Title,
            project.Description,
            project.Status.ToString(),
            project.StartDate,
            project.EndDate,
            project.Owner,
            project.CompanyId);
    }

    /// <summary>
    /// Maps a ProjectModel to a ProjectResponse with related data.
    /// </summary>
    /// <param name="project">The project model.</param>
    /// <returns>The project response with related data.</returns>
    public static ProjectResponse MapToProjectResponseWithDetails(ProjectModel project)
    {
        return new ProjectResponse(
            project.Id,
            project.Title,
            project.Description,
            project.Status.ToString(),
            project.StartDate,
            project.EndDate,
            project.Owner,
            project.CompanyId,
            project.Statuses.Select(s => new ProjectStatusResponse(s.Id, s.Name, s.Color, s.Order, s.IsFinal)).ToList(),
            project.Tasks.Select(t => new ProjectTaskResponse(
                t.Id, t.Title, t.Description, t.Priority.ToString(), t.Type.ToString(), t.EstimatePoints, t.DueDate)).ToList());
    }
}
