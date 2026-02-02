using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database.Requests;

public class OrderDetailRequest
{
    [Required(ErrorMessage = "Module is required")]
    public Guid ModuleId
    {
        get; set;
    }
}
