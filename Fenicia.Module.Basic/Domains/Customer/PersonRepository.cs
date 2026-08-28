using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Customer;

public class PersonRepository(DefaultContext context) : Repository<PersonModel>(context)
{
}
