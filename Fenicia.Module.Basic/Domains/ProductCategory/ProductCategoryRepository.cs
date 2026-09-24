using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

/// <summary>
/// Repository implementation for managing product category data.
/// </summary>
/// <param name="context">The database context.</param>
public class ProductCategoryRepository(DbContext context)
    : Repository<ProductCategoryModel>(context), IProductCategoryRepository
{
}