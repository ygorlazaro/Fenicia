using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Upload.Interfaces;

/// <summary>
/// Repository interface for managing file uploads in the authentication domain.
/// </summary>
public interface IUploadRepository : IRepository<UploadModel>;