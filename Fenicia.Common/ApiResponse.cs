using System.Net;

namespace Fenicia.Common;

public class ApiResponse<T>(
    T? data,
    HttpStatusCode status = HttpStatusCode.OK,
    string message = ""
)
{
    public T? Data { get; set; } = data;
    public HttpStatusCode Status { get; set; } = status;
    public string Message { get; set; } = message;
}
