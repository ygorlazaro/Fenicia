using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Web.Providers.Auth;

public class SignUpProvider : BaseProvider
{
    public SignUpProvider(IConfiguration configuration) : base(configuration)
    {

    }

    public async Task<UserResponse> SignUpAsync(UserRequest request)
    {
        return await base.PostAsync<UserResponse, UserRequest>("signup", request);
    }
}
