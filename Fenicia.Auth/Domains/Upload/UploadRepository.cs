using Fenicia.Auth.Domains.Upload.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Upload;

public class UploadRepository(DbContext context) : Repository<UploadModel>(context), IUploadRepository;