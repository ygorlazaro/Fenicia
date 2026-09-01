using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonRepository(DefaultContext context) : Repository<PersonModel>(context), IPersonRepository;
