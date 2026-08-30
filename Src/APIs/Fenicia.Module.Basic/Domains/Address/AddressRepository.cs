using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Address;

public class AddressRepository(DefaultContext context) : Repository<AddressModel>(context), IAddressRepository
{
}
