using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierRepository(BasicContext context) : BaseRepository<SupplierModel>(context), ISupplierRepository
{
}
