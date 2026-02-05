using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Supplier;

public class SupplierRepository(BasicContext context) : BaseRepository<SupplierModel>(context), ISupplierRepository
{
}
