namespace Fenicia.Common.Database.Responses;

using Models.Auth;

[Serializable]
public class ForgotPasswordResponse
{
    public Guid Id
    {
        get; set;
    }

    public static ForgotPasswordResponse Convert(ForgotPasswordModel model)
    {
        return new ForgotPasswordResponse
        {
            Id = model.Id
        };
    }
}
