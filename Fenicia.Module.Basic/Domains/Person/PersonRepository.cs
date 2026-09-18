using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Basic.Domains.Person.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Person;

public class PersonRepository(DbContext context) : Repository<PersonModel>(context), IPersonRepository;