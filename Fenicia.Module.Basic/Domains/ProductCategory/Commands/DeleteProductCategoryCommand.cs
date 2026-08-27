using MediatR;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Commands;

public record DeleteProductCategoryCommand(

    Guid Id) : IRequest;