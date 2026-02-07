namespace Fenicia.Common;

public static class TextConstants
{
    public const string InvalidJwtSecretMessage = "Invalid Jwt Secret";
    public const string InvalidUsernameOrPasswordMessage = "Invalid username or password";
    public const string PermissionDeniedMessage = "Permission denied";
    public const string ThereWasAnErrorSearchingModulesMessage = "Houve um problema buscando os módulos";

    public const string ThereWasAnErrorAddingModulesMessage =
        "Ocorreu um problema para adicionar créditos de assinatura";

    public const string EmailExistsMessage = "This email exists";
    public const string CompanyExistsMessage = "This company exists";
    public const string MissingAdminRoleMessage = "Missing admin role";
    public const string UserNotInCompanyMessage = "User not in company";
    public const string ItemNotFoundMessage = "Item not found";
    public const string InvalidPasswordMessage = "Invalid password";
    public const string ResetPasswordCodeNotFoundMessage = "Reset password code not found";
    public static string ThereWasAnErrorEditingMessage = "There was an error editing ";

    public static string InvalidForgetCode = "Invalid code";

    public static string UserDoestNotExistsAtTheCompany = "User does not exists at the company";
    public static string ModulesNotFound = "Module not found";

    public static string TooManyAttempts = "Too many login attempts. Try again later";
}