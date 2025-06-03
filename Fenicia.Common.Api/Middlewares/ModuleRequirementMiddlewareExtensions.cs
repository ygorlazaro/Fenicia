using Microsoft.AspNetCore.Builder;

namespace Fenicia.Common.Api.Middlewares;

public static class ModuleRequirementMiddlewareExtensions
{
    public static IApplicationBuilder UseModuleRequirement(this IApplicationBuilder builder, string moduleName)
    {
        return builder.UseMiddleware<ModuleRequirementMiddleware>(moduleName);
    }
}
