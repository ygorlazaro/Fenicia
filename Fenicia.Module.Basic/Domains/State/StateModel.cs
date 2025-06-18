using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Database;
using Fenicia.Module.Basic.Domains.Address;

namespace Fenicia.Module.Basic.Domains.State;

[Table("states")]
public class StateModel : BaseModel
{
    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(2)]
    public string Uf { get; set; } = null!;

    public virtual List<AddressModel> Addresses { get; set; } = null!;
}
