using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Basic;

[Table("states")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    public virtual List<CustomerModel> Customers { get; set; }

    public virtual List<SupplierModel> Suppliers { get; set; }

    public virtual List<EmployeeModel> Employees { get; set; }
}