using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Basic;

public class EmployeeRequest
{
    public Guid Id
    {
        get;
        set;
    }

    [Required]
    public Guid PositionId
    {
        get;
        set;
    }

    public PersonRequest Person
    {
        get;
        set;
    }
}
