using Fenicia.Common.API;
using Fenicia.Common.API.Startup;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project;
using Fenicia.Module.Projects.Domains.ProjectAttachment;
using Fenicia.Module.Projects.Domains.ProjectComment;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectSubtask;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTaskAssignee;

namespace Fenicia.Module.Projects;

public abstract class Program
{
    public static void Main(string[] args)
    {
        FeniciaModuleLoader.Load(args, out var configuration, out var builder);

        builder.AddFeniciaLogging().AddFeniciaRateLimiting(configuration).AddFeniciaCors().AddFeniciaAuthentication(configuration).AddFeniciaControllers().AddFeniciaDependencyInjection(() =>
    {
        builder.Services.AddSingleton<ICompanyContext, CompanyContext>();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ProjectRepository>();
        builder.Services.AddScoped<ProjectAttachmentRepository>();
        builder.Services.AddScoped<ProjectCommentRepository>();
        builder.Services.AddScoped<ProjectSubtaskRepository>();
        builder.Services.AddScoped<ProjectTaskRepository>();
        builder.Services.AddScoped<ProjectTaskAssigneeRepository>();
        builder.Services.AddScoped<ProjectStatusRepository>();
    }).AddFeniciaDbContext<DefaultContext>(configuration, "Fenicia.Auth", "Auth");

        builder.Start("/projects", "projects");
    }
}
