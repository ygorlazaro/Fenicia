using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Common.Database.Responses;

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
