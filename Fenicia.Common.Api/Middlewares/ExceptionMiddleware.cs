using System.Diagnostics;
using System.Globalization;
using Fenicia.Common.Data;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace Fenicia.Common.API.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ICompanyContext companyContext)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            await next(context);
        }
#pragma warning disable CA1031
        catch (Exception ex)
        {
#pragma warning restore CA1031
            sw.Stop();

            var acceptLanguage = context.Request.Headers.AcceptLanguage.FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptLanguage))
            {
                SetCulture(acceptLanguage);
            }

            var (statusCode, errorCode) = GetStatusCodeAndErrorCode(ex);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var userId = GetUserId(context);
            var companyId = GetCompanyId();

            var response = new
            {
                message = ex.Message,
                route = context.Request.Path.Value,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                ms = sw.ElapsedMilliseconds,
                statusCode,
                errorCode,
                userId,
                companyId
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static (int statusCode, string errorCode) GetStatusCodeAndErrorCode(Exception ex)
    {
        return ex switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized"),
            InvalidRequestException => (StatusCodes.Status400BadRequest, "InvalidRequest"),
            ItemNotExistsException => (StatusCodes.Status404NotFound, "ItemNotFound"),
            PermissionDeniedException => (StatusCodes.Status403Forbidden, "PermissionDenied"),
            InvalidDataException => (StatusCodes.Status400BadRequest, "InvalidData"),
            NotSavedException => (StatusCodes.Status500InternalServerError, "NotSaved"),
            _ => (StatusCodes.Status500InternalServerError, "InternalError")
        };
    }

    private Guid? GetUserId(HttpContext context)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == "userId");
        return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
    }

    private Guid? GetCompanyId()
    {
        return companyContext.CompanyId != Guid.Empty ? companyContext.CompanyId : null;
    }

    private static void SetCulture(string acceptLanguage)
    {
        var primaryLanguage = acceptLanguage.Split(',').FirstOrDefault()?.Split(';').FirstOrDefault()?.Trim();

        if (string.IsNullOrEmpty(primaryLanguage))
        {
            return;
        }

        try
        {
            var culture = new CultureInfo(primaryLanguage);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
        catch (CultureNotFoundException)
        {
            var culture = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
