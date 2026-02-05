using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryRepository(BasicContext context) : BaseRepository<ProductCategoryModel>(context), IProductCategoryRepository
{
}
