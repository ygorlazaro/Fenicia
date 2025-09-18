namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class OrderDetailRequest
{
    [Required(ErrorMessage = "Module is required")]
    public Guid ModuleId
    {
        get; set;
    }
}
