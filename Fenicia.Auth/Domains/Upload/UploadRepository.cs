using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Upload;

/// <summary>
/// Repository implementation for managing file uploads in the authentication domain.
/// </summary>
/// <param name="context">The database context.</param>
public class UploadRepository(DbContext context) : Repository<UploadModel>(context), IUploadRepository;