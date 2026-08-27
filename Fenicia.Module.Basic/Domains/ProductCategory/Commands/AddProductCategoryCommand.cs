using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

public record AddProductCategoryCommand(

    Guid Id,

    string Name) : IRequest<AddProductCategoryResponse>;