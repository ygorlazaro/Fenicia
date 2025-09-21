using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Web.Providers.Auth;

using Abstracts;

public class RegisterProvider : BaseProvider
{
    public RegisterProvider(IConfiguration configuration, AuthManager authManager) : base(configuration, authManager)
    {

    }

    public async Task<UserResponse> RegisterAsync(UserRequest request)
    {
        return await PostAsync<UserResponse, UserRequest>("register", request);
    }
}
