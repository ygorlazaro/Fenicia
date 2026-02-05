using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Common.Data.Responses.Basic;

public class AddressResponse
{
    public Guid Id
    {
        get;
        set;
    }

    public string Street
    {
        get;
        set;
    }

    public string Number
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

    public string ZipCode
    {
        get;
        set;
    }

    public StateModel StateId
    {
        get;
        set;
    }

    public string City
    {
        get;
        set;
    }
}
