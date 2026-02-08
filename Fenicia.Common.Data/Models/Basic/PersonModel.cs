using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Requests.Basic;

namespace Fenicia.Common.Data.Models.Basic;

[Table("people")]
public class PersonModel(PersonRequest request) : BaseModel
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = request.Name;

    [MaxLength(14)]
    public string? Cpf { get; set; } = request.Cpf;

    [Required]
    [MaxLength(100)]
    public string Street { get; set; } = request.Street;


    [Required]
    [MaxLength(10)]
    public string Number { get; set; } = request.Number;


    [MaxLength(10)]
    public string Complement { get; set; } = request.Complement;


    [MaxLength(50)]
    public string Neighborhood { get; set; } = request.Neighborhood;


    [Required]
    [MaxLength(8)]
    public string ZipCode { get; set; } = request.ZipCode;


    [Required]
    public Guid StateId { get; set; } = request.StateId;

    [Required]
    [MaxLength(50)]
    public string City { get; set; } = request.City;


    [MaxLength(20)]
    public string PhoneNumber { get; set; } = request.PhoneNumber;

    [ForeignKey(nameof(StateId))]
    public virtual StateModel State { get; set; } = null!;

    public virtual CustomerModel? Customer { get; set; }

    public virtual EmployeeModel? Employee { get; set; }
}
