namespace Fenicia.Common;

using System.Net;

public class ErrorResponse
{
    public string? Message
    {
        get; set;
    }
}

public class ApiResponse<T>(T? data, HttpStatusCode status = HttpStatusCode.OK, string message = "")
{
    public T? Data { get; set; } = data;
    public HttpStatusCode Status { get; set; } = status;
    public ErrorResponse Message { get; set; } = new ErrorResponse { Message = message };
}
