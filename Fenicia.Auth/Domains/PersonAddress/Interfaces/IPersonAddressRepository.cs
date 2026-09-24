using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.PersonAddress.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages person address relationship data operations.
/// </summary>
public interface IPersonAddressRepository : IRepository<PersonAddressModel>;