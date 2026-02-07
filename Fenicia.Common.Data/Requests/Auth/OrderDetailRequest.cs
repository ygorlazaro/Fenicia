using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Auth;

public class OrderDetailRequest
{
    [Required(ErrorMessage = "Module is required")]
    public Guid ModuleId
    {
        get; set;
    }
}