using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Basic;

[Table("person_addresses", Schema = "basic")]
public class PersonAddressModel : BaseCompanyModel
{
    public Guid PersonId { get; set; }

    public Guid AddressId { get; set; }

    [ForeignKey(nameof(PersonId))]
    public virtual PersonModel Person { get; set; } = null!;

    [ForeignKey(nameof(AddressId))]
    public virtual AddressModel Address { get; set; } = null!;
}
