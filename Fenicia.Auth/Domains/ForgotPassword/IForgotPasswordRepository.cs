namespace Fenicia.Auth.Domains.ForgotPassword;

public interface IForgotPasswordRepository
{
    Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code);
    Task InvalidateCodeAsync(Guid id);
    public Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPasswordId);
}
