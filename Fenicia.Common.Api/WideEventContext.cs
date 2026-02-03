using System.Diagnostics;

namespace Fenicia.Common.Api;

public sealed class WideEventContext
{
    public string TraceId { get; } = Activity.Current?.TraceId.ToString();

    public string Path { get; set; } = null!;

    public string Method { get; set; } = null!;

    public int StatusCode
    {
        get; set;
    }

    public long DurationMs
    {
        get; set;
    }

    public string? UserId
    {
        get; set;
    }

    public string Operation => $"{Path} {Method}";

    public bool Success
    {
        get; set;
    }

    public string? ErrorCode
    {
        get; set;
    }

    public string? ErrorMessage
    {
        get; set;
    }
}
