using System.Security.Claims;
using Bogus;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Tests;
using Fenicia.Module.Projects.Domains.ProjectTask;
using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Module.Projects.Tests.Domains.ProjectTask;

public class ProjectTaskControllerTests : IDisposable
{
    private readonly ProjectTaskController controller;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Mock<HttpContext> mockHttpContext;
    private readonly Guid testUserId;
    private readonly Guid companyId;

    public ProjectTaskControllerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var repository = new ProjectTaskRepository(db);
        var service = new ProjectTaskService(repository);
        mockHttpContext = new Mock<HttpContext>();
        controller = new ProjectTaskController(service) { ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object } };
        testUserId = Guid.NewGuid();
        companyId = companyContext.CompanyId;
        SetupUserClaims(testUserId);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAsync_WhenTasksExist_ReturnsOk()
    {
        var wide = new WideEventContext();
        
        var result = await controller.GetAsync(wide, 1, 10, CancellationToken.None);
        
        Assert.IsType<OkObjectResult>(result.Result);
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim> { new("userId", userId.ToString()) };
        var claimsIdentity = new ClaimsIdentity(claims, "Test");
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        mockHttpContext.Setup(x => x.User).Returns(claimsPrincipal);
        controller.ControllerContext.HttpContext.User = claimsPrincipal;
    }
}
