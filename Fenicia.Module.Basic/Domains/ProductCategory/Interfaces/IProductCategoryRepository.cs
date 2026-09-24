using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;

/// <summary>
/// Repository interface for managing product category data.
/// </summary>
public interface IProductCategoryRepository : IRepository<ProductCategoryModel>
{
}