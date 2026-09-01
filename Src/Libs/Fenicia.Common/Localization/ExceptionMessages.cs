using System.Globalization;
using System.Resources;

namespace Fenicia.Common.Localization;

public static class ExceptionMessages
{
    private static readonly ResourceManager _resourceMapper = new("Fenicia.Common.Resources.ExceptionMessages", typeof(ExceptionMessages).Assembly);

    public static string InvalidRequest => GetString("InvalidRequest");

    public static string ItemNotFound => GetString("ItemNotFound");

    public static string ItemNotExists => GetString("ItemNotExists");

    public static string NotSaved => GetString("NotSaved");

    public static string PermissionDenied => GetString("PermissionDenied");

    public static string UserNotFound => GetString("UserNotFound");

    public static string UserWithEmailNotFound => GetString("UserWithEmailNotFound");

    public static string EmailAlreadyExists => GetString("EmailAlreadyExists");

    public static string RoleNotFound => GetString("RoleNotFound");

    public static string CompanyNotFoundWithCNPJ => GetString("CompanyNotFoundWithCNPJ");

    public static string AdminRoleNotFound => GetString("AdminRoleNotFound");

    public static string TooManyLoginAttempts => GetString("TooManyLoginAttempts");

    public static string InvalidUsernameOrPassword => GetString("InvalidUsernameOrPassword");

    public static string InvalidRefreshToken => GetString("InvalidRefreshToken");

    public static string PasswordCannotBeNullOrEmpty => GetString("PasswordCannotBeNullOrEmpty");

    public static string ErrorHashingPassword => GetString("ErrorHashingPassword");

    public static string InvalidForgotPasswordCode => GetString("InvalidForgotPasswordCode");

    public static string UserNotAssociatedWithActiveCompanies => GetString("UserNotAssociatedWithActiveCompanies");

    public static string UserDoesNotExistsAtCompany => GetString("UserDoesNotExistsAtCompany");

    public static string ModulesNotFound => GetString("ModulesNotFound");

    public static string Unauthorized => GetString("Unauthorized");

    public static string CompanyNotFoundMessage => GetString("CompanyNotFoundById");

    public static string PermissionDeniedUpdateCompany => GetString("PermissionDeniedUpdateCompany");

    public static string CompanyExists => GetString("CompanyExists");

    private static string GetString(string name)
    {
#pragma warning disable CA1304
        return _resourceMapper.GetString(name, CultureInfo.InvariantCulture) ?? name;
#pragma warning restore CA1304
    }
}