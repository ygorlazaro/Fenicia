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

    public static string CompanyNotFound => GetString("CompanyNotFound");

    public static string RoleNotFound => GetString("RoleNotFound");

    public static string CompanyNotFoundWithCNPJ => GetString("CompanyNotFoundWithCNPJ");

    public static string AdminRoleNotFound => GetString("AdminRoleNotFound");

    public static string TooManyLoginAttempts => GetString("TooManyLoginAttempts");

    public static string InvalidUsernameOrPassword => GetString("InvalidUsernameOrPassword");

    public static string InvalidRefreshToken => GetString("InvalidRefreshToken");

    public static string PasswordCannotBeNullOrEmpty => GetString("PasswordCannotBeNullOrEmpty");

    public static string ErrorHashingPassword => GetString("ErrorHashingPassword");

    public static string InvalidPassword => GetString("InvalidPassword");

    public static string InvalidForgotPasswordCode => GetString("InvalidForgotPasswordCode");

    public static string UserNotAssociatedWithActiveCompanies => GetString("UserNotAssociatedWithActiveCompanies");

    public static string UserDoesNotExistsAtCompany => GetString("UserDoesNotExistsAtCompany");

    public static string ModulesNotFound => GetString("ModulesNotFound");

    public static string OrderDetailsCannotBeEmpty => GetString("OrderDetailsCannotBeEmpty");

    public static string Unauthorized => GetString("Unauthorized");

    public static string InternalError => GetString("InternalError");

    public static string CompanyNotFoundMessage => GetString("CompanyNotFoundById");

    public static string PermissionDeniedUpdateCompany => GetString("PermissionDeniedUpdateCompany");

    public static string CompanyExists => GetString("CompanyExists");

    public static string UserWithIdNotFound(string userId)
    {
        return string.Format(GetString("UserWithIdNotFound"), userId);
    }

    public static string CompanyNotFoundById(string companyId)
    {
        return string.Format(GetString("CompanyNotFound"), companyId);
    }

    public static string RoleNotFoundById(string roleId)
    {
        return string.Format(GetString("RoleNotFound"), roleId);
    }

    public static string GetStringWithFormat(string key, params object[] args)
    {
        var format = GetString(key);
        return string.Format(format, args);
    }

    private static string GetString(string name)
    {
        return _resourceMapper.GetString(name) ?? name;
    }
}
