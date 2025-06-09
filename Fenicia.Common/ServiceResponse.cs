using System.Net;

namespace Fenicia.Common;

public class ServiceResponse<T>(
    T? data,
    HttpStatusCode statusCode = HttpStatusCode.OK,
    string message = ""
)
{
    public T? Data { get; set; } = data;
    public HttpStatusCode StatusCode { get; set; } = statusCode;
    public string Message { get; set; } = message;
}
