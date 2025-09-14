namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public class OrderDetailRequest
{
    [Required(ErrorMessage = "Module ID is required")]
    [Display(Name = "Module ID")]
    public Guid ModuleId
    {
        get; set;
    }
}
