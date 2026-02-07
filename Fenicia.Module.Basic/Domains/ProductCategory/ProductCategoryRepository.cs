using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryRepository(BasicContext context)
    : BaseRepository<ProductCategoryModel>(context), IProductCategoryRepository
{
}