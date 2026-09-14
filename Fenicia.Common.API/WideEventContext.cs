using System.Diagnostics;

namespace Fenicia.Common.API;

public class WideEventContext
{
    public string TraceId { get; } = Activity.Current?.TraceId.ToString() ?? string.Empty;

    public string Path { get; set; } = string.Empty;

    public string Method { get; set; } = string.Empty;

    public int StatusCode { get; set; }

    public long DurationMs { get; set; }

    public string? UserId { get; set; }

    public string Operation => $"{Path} {Method}";

    public bool Success { get; set; }

    public string? ErrorCode { get; set; }

    public string? ErrorMessage { get; set; }
}