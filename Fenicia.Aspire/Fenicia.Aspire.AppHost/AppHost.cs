var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure
var cache = builder.AddRedis("cache");
builder.AddPostgres("postgres");

// Authentication & Authorization API
var auth = builder.AddProject<Projects.Fenicia_Auth>("authapi")
    .WithReference(cache)
    .WaitFor(cache);

// Module APIs — all use fallback connection string from appsettings.Common.json
var basic = builder.AddProject<Projects.Fenicia_Module_Basic>("basic");
builder.AddProject<Projects.Fenicia_Module_Accounting>("accounting");
builder.AddProject<Projects.Fenicia_Module_Contracts>("contracts");
builder.AddProject<Projects.Fenicia_Module_HR>("hr");
builder.AddProject<Projects.Fenicia_Module_PerformanceEvaluation>("performanceevaluation");
builder.AddProject<Projects.Fenicia_Module_Plus>("plus");
builder.AddProject<Projects.Fenicia_Module_POS>("pos");

var customerSupport = builder.AddProject<Projects.Fenicia_Module_CustomerSupport>("customersupport")
    .WithReference(cache);

builder.AddProject<Projects.Fenicia_Module_Ecommerce>("ecommerce")
    .WithReference(cache);

var projectModule = builder.AddProject<Projects.Fenicia_Module_Projects>("projects");
var socialNetwork = builder.AddProject<Projects.Fenicia_Module_SocialNetwork>("socialnetwork")
    .WithReference(cache);

// Fenicia Web frontend — uses Aspire service discovery for API URLs
builder.AddProject<Projects.Fenicia_Web>("fenicia-web")
    .WithExternalHttpEndpoints()
    .WithReference(auth)
    .WithReference(basic)
    .WithReference(socialNetwork)
    .WithReference(projectModule)
    .WaitFor(auth)
    .WaitFor(basic)
    .WaitFor(socialNetwork)
    .WaitFor(projectModule);

// Existing Aspire template resources
var apiService = builder.AddProject<Projects.Fenicia_Aspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Fenicia_Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
