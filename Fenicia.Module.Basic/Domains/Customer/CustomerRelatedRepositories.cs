using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer;

public class PersonRepository(DefaultContext context) : Repository<PersonModel>(context)
{
}

public class AddressRepository(DefaultContext context) : Repository<AddressModel>(context)
{
}

public class PersonAddressRepository(DefaultContext context) : Repository<PersonAddressModel>(context)
{
}
