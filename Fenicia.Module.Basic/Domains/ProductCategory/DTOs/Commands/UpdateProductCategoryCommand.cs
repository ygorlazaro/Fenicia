using Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Commands;

public record UpdateProductCategoryCommand(

    Guid Id,

    string Name);