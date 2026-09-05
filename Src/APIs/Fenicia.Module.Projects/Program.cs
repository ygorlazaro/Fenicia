using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.Project.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectAttachment.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectComment.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;
using Fenicia.Module.Projects.Domains.Team;
using Fenicia.Module.Projects.Domains.Team.Interfaces;

namespace Fenicia.Module.Projects;

public abstract class Program
{
    public static void Main(string[] args)
    {
        FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors()
            .AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaDependencyInjection(() =>
            {
                builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
                builder.Services.AddHttpContextAccessor();
                builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
                builder.Services.AddScoped<IRepository<ProjectModel>, ProjectRepository>();
                builder.Services.AddScoped<IRepository<AttachmentModel>, ProjectAttachmentRepository>();
                builder.Services.AddScoped<IRepository<ProjectCommentModel>, ProjectCommentRepository>();
                builder.Services.AddScoped<IRepository<ProjectSubtaskModel>, ProjectSubtaskRepository>();
                builder.Services.AddScoped<IProjectTaskRepository, ProjectTaskRepository>();
                builder.Services.AddScoped<IRepository<TaskAssigneeModel>, ProjectTaskAssigneeRepository>();
                builder.Services.AddScoped<IProjectStatusRepository, ProjectStatusRepository>();
                builder.Services.AddScoped<IProjectService, ProjectService>();
                builder.Services.AddScoped<IProjectAttachmentService, ProjectAttachmentService>();
                builder.Services.AddScoped<IProjectCommentService, ProjectCommentService>();
                builder.Services.AddScoped<IProjectSubtaskService, ProjectSubtaskService>();
                builder.Services.AddScoped<IProjectTaskService, ProjectTaskService>();
                builder.Services.AddScoped<IProjectTaskAssigneeService, ProjectTaskAssigneeService>();
                builder.Services.AddScoped<IProjectStatusService, ProjectStatusService>();
                builder.Services.AddScoped<ITeamRepository, TeamRepository>();
                builder.Services.AddScoped<ITeamUserRepository, TeamUserRepository>();
                builder.Services.AddScoped<ITeamService, TeamService>();
            }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        builder.Start("/projects", "projects");
    }
}