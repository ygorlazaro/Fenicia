namespace Fenicia.Common.Database.Models.Basic;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Database;

[Table("states")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(length: 30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(length: 2)]
    public string Uf { get; set; } = null!;

    public virtual List<AddressModel> Addresses { get; set; } = null!;
}
