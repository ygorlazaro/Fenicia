namespace Fenicia.Web.Services;

public class CrudApiException(string message, System.Net.HttpStatusCode statusCode) : Exception(message)
{
    public System.Net.HttpStatusCode StatusCode { get; } = statusCode;
}