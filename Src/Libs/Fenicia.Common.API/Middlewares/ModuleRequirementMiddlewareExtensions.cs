using Microsoft.AspNetCore.Builder;

namespace Fenicia.Common.API.Middlewares;

public static class ModuleRequirementMiddlewareExtensions
{
    public static void UseModuleRequirement(this IApplicationBuilder builder, string moduleName)
    {
        builder.UseMiddleware<ModuleRequirementMiddleware>(moduleName);
    }
}
