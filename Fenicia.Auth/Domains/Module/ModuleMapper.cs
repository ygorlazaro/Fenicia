using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Module;

namespace Fenicia.Auth.Domains.Module;

public static class ModuleMapper
{
    /// <summary>
    /// Maps a ModuleModel to a ModuleResponse DTO.
    /// </summary>
    /// <param name="module">The module model to map.</param>
    /// <returns>The module response DTO.</returns>
    public static ModuleResponse MapToModuleResponse(ModuleModel module)
    {
        return new ModuleResponse(
            module.Id,
            module.Name,
            module.Type,
            module.Description,
            module.IsActive,
            module.SortOrder,
            module.Price);
    }

    /// <summary>
    /// Maps a ModuleModel to a ModuleByUserResponse DTO.
    /// </summary>
    /// <param name="module">The module model to map.</param>
    /// <returns>The module by user response DTO.</returns>
    public static ModuleByUserResponse MapToModuleByUserResponse(ModuleModel module)
    {
        return new ModuleByUserResponse(module.Id, module.Name, module.Type);
    }

    public static ModuleModel MapToModuleModel(ModuleResponse source)
    {
        return new ModuleModel
        {
            Id = source.Id,
            Name = source.Name,
            Type = source.Type,
            Description = source.Description,
            IsActive = source.IsActive,
            SortOrder = source.SortOrder,
            Price = source.Price ?? 0
        };
    }
}
