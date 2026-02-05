namespace Fenicia.Common.Database.Responses.Basic;

public class CustomerResponse
{
    public Guid Id
    {
        get;
        set;
    }

    public string? Cpf
    {
        get;
        set;
    }

    public string Name
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
