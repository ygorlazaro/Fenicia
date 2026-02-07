namespace Fenicia.Common.Data.Responses.Basic;

public class EmployeeResponse
{
    public Guid Id
    {
        get;
        set;
    }

    public Guid PositionId
    {
        get;
        set;
    }

    public PersonResponse Person
    {
        get;
        set;
    }
}
