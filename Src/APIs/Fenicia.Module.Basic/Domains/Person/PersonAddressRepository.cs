using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonAddressRepository(DefaultContext context) : Repository<PersonAddressModel>(context)
{
}
