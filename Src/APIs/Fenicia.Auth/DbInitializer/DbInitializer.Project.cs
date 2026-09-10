using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Auth.DbInitializer;

internal static partial class DbInitializer
{
    private static void SeedProjects(DefaultContext context)
    {
        if (context.Projects.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var users = context.AuthUsers.ToList();

        if (!users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 20; i++)
        {
            var startDate = now.AddDays(-(random.NextDouble() * 365));
            var endDate = now.AddDays(random.NextDouble() * 180);

            var title = (i % 5) switch
            {
                0 => $"Projeto {i} — Plataforma E-commerce",
                1 => $"Projeto {i} — App Mobile",
                2 => $"Projeto {i} — Sistema de CRM",
                3 => $"Projeto {i} — Portal do Cliente",
                _ => $"Projeto {i} — Integração API"
            };

            var project = new ProjectModel
            {
                CompanyId = companyId,
                Title = title,
                Description = $"Descrição do projeto {i} com escopo, objetivos e entregas definidas.",
                Status = (EnumProjectStatus)(i % 4),
                StartDate = startDate,
                EndDate = endDate,
                Owner = users[(i - 1) % users.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.Projects.Add(project);
        }
    }

    private static void SeedProjectStatuses(DefaultContext context)
    {
        if (context.ProjectStatuses.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var projects = context.Projects.ToList();

        if (!projects.Any())
        {
            return;
        }

        var statuses = new[]
        {
            new { Name = "Backlog", Color = "#6c757d", Order = 1, IsFinal = false },
            new { Name = "A Fazer", Color = "#0dcaf0", Order = 2, IsFinal = false },
            new { Name = "Em Progresso", Color = "#ffc107", Order = 3, IsFinal = false },
            new { Name = "Em Revisão", Color = "#fd7e14", Order = 4, IsFinal = false },
            new { Name = "Concluído", Color = "#198754", Order = 5, IsFinal = true },
            new { Name = "Bloqueado", Color = "#dc3545", Order = 6, IsFinal = false }
        };

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 60; i++)
        {
            var statusIndex = (i - 1) % 6;
            var project = projects[(i - 1) % projects.Count];

            var projectStatus = new ProjectStatusModel
            {
                CompanyId = companyId,
                ProjectId = project.Id,
                Name = statuses[statusIndex].Name,
                Color = statuses[statusIndex].Color,
                Order = statuses[statusIndex].Order,
                IsFinal = statuses[statusIndex].IsFinal,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectStatuses.Add(projectStatus);
        }
    }

    private static void SeedSprints(DefaultContext context)
    {
        if (context.Sprints.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var projects = context.Projects.ToList();
        var users = context.AuthUsers.ToList();

        if (!projects.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 40; i++)
        {
            var startDate = now.AddDays(-(random.NextDouble() * 180));
            var endDate = now.AddDays(random.NextDouble() * 180);

            var sprint = new SprintModel
            {
                CompanyId = companyId,
                ProjectId = projects[(i - 1) % projects.Count].Id,
                Name = $"Sprint {i}",
                Description = $"Sprint {i} do projeto {(i % 20) + 1}",
                StartDate = startDate,
                EndDate = endDate,
                CreatedBy = users[(i - 1) % users.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 180),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.Sprints.Add(sprint);
        }
    }

    private static void SeedTasks(DefaultContext context)
    {
        if (context.ProjectTasks.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var projects = context.Projects.ToList();
        var projectStatuses = context.ProjectStatuses.ToList();
        var users = context.AuthUsers.ToList();
        var sprints = context.Sprints.ToList();

        if (!projects.Any() || !projectStatuses.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var dueDate = now.AddDays(random.NextDouble() * 90);
            var sprintId = i % 2 == 0 && sprints.Any() ? (Guid?)sprints[(i - 1) % sprints.Count].Id : null;

            var title = (i % 5) switch
            {
                0 => $"Task {i} — Implementar login",
                1 => $"Task {i} — Corrigir bug no checkout",
                2 => $"Task {i} — Criar dashboard",
                3 => $"Task {i} — Otimizar query",
                _ => $"Task {i} — Refatorar módulo"
            };

            var task = new ProjectTaskModel
            {
                CompanyId = companyId,
                ProjectId = projects[(i - 1) % projects.Count].Id,
                StatusId = projectStatuses[(i - 1) % projectStatuses.Count].Id,
                Title = title,
                Description = $"Descrição da task {i} com detalhes de implementação.",
                Priority = (EnumTaskPriority)(i % 4),
                Type = (EnumTaskType)(i % 5),
                Order = i,
                EstimatePoints = (i % 13) + 1,
                DueDate = dueDate,
                CreatedBy = users[(i - 1) % users.Count].Id,
                SprintId = sprintId,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectTasks.Add(task);
        }
    }

    private static void SeedProjectSubtasks(DefaultContext context)
    {
        if (context.ProjectSubtasks.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var tasks = context.ProjectTasks.ToList();

        if (!tasks.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 200; i++)
        {
            var dueDate = now.AddDays(random.NextDouble() * 60);
            var isCompleted = i % 3 == 0;
            var completedAt = isCompleted ? (DateTime?)now.AddDays(-(random.NextDouble() * 30)) : null;

            var subtask = new ProjectSubtaskModel
            {
                CompanyId = companyId,
                TaskId = tasks[(i - 1) % tasks.Count].Id,
                Title = $"Subtask {i} de {(i % 100) + 1}",
                Order = (i % 5) + 1,
                IsCompleted = isCompleted,
                DueDate = dueDate,
                CompletedAt = completedAt,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectSubtasks.Add(subtask);
        }
    }

    private static void SeedTaskAssignees(DefaultContext context)
    {
        if (context.ProjectTaskAssignees.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var tasks = context.ProjectTasks.ToList();
        var users = context.AuthUsers.ToList();

        if (!tasks.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 200; i++)
        {
            var assignedAt = now.AddDays(-(random.NextDouble() * 365));

            var taskAssignee = new TaskAssigneeModel
            {
                CompanyId = companyId,
                TaskId = tasks[(i - 1) % tasks.Count].Id,
                UserId = users[(i - 1) % users.Count].Id,
                Role = (EnumAssigneeRole)(i % 3),
                AssignedAt = assignedAt,
                Created = now.AddDays(-random.NextDouble() * 30),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectTaskAssignees.Add(taskAssignee);
        }
    }

    private static void SeedProjectComments(DefaultContext context)
    {
        if (context.ProjectComments.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var tasks = context.ProjectTasks.ToList();
        var users = context.AuthUsers.ToList();

        if (!tasks.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 100; i++)
        {
            var projectComment = new ProjectCommentModel
            {
                CompanyId = companyId,
                TaskId = tasks[(i - 1) % tasks.Count].Id,
                UserId = users[(i - 1) % users.Count].Id,
                AuthorId = users[(i - 1) % users.Count].Id,
                Content = $"Comentário no task {(i % 100) + 1} pelo usuário {(i % 100) + 1}.",
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectComments.Add(projectComment);
        }
    }

    private static void SeedProjectAttachments(DefaultContext context)
    {
        if (context.ProjectAttachments.Any())
        {
            return;
        }

        var companyId = context.AuthCompanies.Select(x => x.Id).FirstOrDefault();
        var tasks = context.ProjectTasks.ToList();
        var users = context.AuthUsers.ToList();

        if (!tasks.Any() || !users.Any())
        {
            return;
        }

        var random = new Random();
        var now = DateTime.UtcNow;

        for (var i = 1; i <= 50; i++)
        {
            var fileSize = (long)((random.NextDouble() * 5000000) + 500000);

            var attachment = new AttachmentModel
            {
                CompanyId = companyId,
                TaskId = tasks[(i - 1) % tasks.Count].Id,
                FileName = $"anexo-{i}.pdf",
                FileUrl = $"https://fenicia.s3.amazonaws.com/anexos/anexo-{i}.pdf",
                FileSize = fileSize,
                Size = fileSize,
                ContentType = "application/pdf",
                UploadedBy = users[(i - 1) % users.Count].Id,
                Created = now.AddDays(-random.NextDouble() * 365),
                Updated = now.AddDays(-random.NextDouble() * 30)
            };

            context.ProjectAttachments.Add(attachment);
        }
    }
}
