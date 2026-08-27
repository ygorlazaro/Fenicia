using Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.DTOs.Commands;

public record AddProductCategoryCommand(

    Guid Id,

    string Name);