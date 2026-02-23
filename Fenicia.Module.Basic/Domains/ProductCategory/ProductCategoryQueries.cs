namespace Fenicia.Module.Basic.Domains.ProductCategory;

public record GetAllProductCategoryQuery;
public record GetProductCategoryByIdQuery(Guid Id);
public record AddProductCategoryCommand(Guid Id, string Name);
public record UpdateProductCategoryCommand(Guid Id, string Name);
public record DeleteProductCategoryCommand(Guid Id);

public record ProductCategoryResponse(Guid Id, string Name);
