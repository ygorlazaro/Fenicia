using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data;

public abstract class BaseCompanyModel : BaseModel
{
    [Required]
    public Guid CompanyId { get; set; }
}