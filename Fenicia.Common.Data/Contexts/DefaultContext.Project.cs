using Fenicia.Common.Data.Models.ProjectModels;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Common.Data.Contexts;

public partial class DefaultContext
{
    public DbSet<ProjectModel> Projects { get; set; }

    public DbSet<ProjectStatusModel> ProjectStatuses { get; set; }

    public DbSet<ProjectTaskModel> ProjectTasks { get; set; }

    public DbSet<ProjectSubtaskModel> ProjectSubtasks { get; set; }

    public DbSet<ProjectCommentModel> ProjectComments { get; set; }

    public DbSet<AttachmentModel> ProjectAttachments { get; set; }

    public DbSet<TaskAssigneeModel> ProjectTaskAssignees { get; set; }
}
