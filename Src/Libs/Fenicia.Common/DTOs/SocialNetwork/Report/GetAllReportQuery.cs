namespace Fenicia.Common.DTOs.SocialNetwork.Report;

public record GetAllReportQuery(
    int Page = 1,
    int PerPage = 10,
    string? Query = null,
    string? Sort = null);