namespace Fenicia.Common.Database.Requests.Basic;

public class EmployeeRequest
{
    public Guid Id
    {
        get;
        set;
    }

    public string Name
    {
        get;
        set;
    }

    public string? Cpf
    {
        get;
        set;
    }

    public Guid AddressId
    {
        get;
        set;
    }

    public Guid PositionId
    {
        get;
        set;
    }

    public string City
    {
        get;
        set;
    }

    public string Complement
    {
        get;
        set;
    }

    public string Neighborhood
    {
        get;
        set;
    }

    public string Number
    {
        get;
        set;
    }

    public Guid StateId
    {
        get;
        set;
    }

    public string Street
    {
        get;
        set;
    }

    public string ZipCode
    {
        get;
        set;
    }
}
