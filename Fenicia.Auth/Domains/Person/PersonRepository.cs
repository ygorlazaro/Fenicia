using Fenicia.Auth.Domains.Person.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Person;

/// <summary>
/// Repository implementation for managing person data.
/// </summary>
/// <param name="context">The database context.</param>
public class PersonRepository(DbContext context) : Repository<PersonModel>(context), IPersonRepository;