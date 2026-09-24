using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Basic.Employee;

public class DeleteEmployeeRequest()
{
    public DeleteEmployeeRequest(Guid id)
        : this()
    {
        Id = id;
    }

    [Required]
    public Guid Id { get; set; }
}
