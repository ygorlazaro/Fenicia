using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Person.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages person-related data operations.
/// </summary>
public interface IPersonRepository : IRepository<PersonModel>;