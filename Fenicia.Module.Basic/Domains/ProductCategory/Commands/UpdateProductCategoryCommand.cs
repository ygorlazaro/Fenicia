using MediatR;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

public record UpdateProductCategoryCommand(

    Guid Id,

    string Name) : IRequest<UpdateProductCategoryResponse?>;