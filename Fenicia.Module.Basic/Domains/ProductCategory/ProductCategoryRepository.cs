using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryRepository(DbContext context)
    : Repository<ProductCategoryModel>(context), IProductCategoryRepository;