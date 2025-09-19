namespace Fenicia.Common;

using System.Net;

public class ErrorResponse
{
    public string? Message
    {
        get; set;
    }
}

public class ApiResponse<T>
{
    public ApiResponse(T? data, HttpStatusCode status = HttpStatusCode.OK, string? message = "")
    {
        this.Data = data;
        this.Status = status;
        this.Message = new ErrorResponse { Message = message };
    }

    public T? Data
    {
        get; set;
    }

    public HttpStatusCode Status
    {
        get; set;
    }

    public ErrorResponse Message
    {
        get; set;
    }
}
