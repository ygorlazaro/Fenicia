using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class UserProvider(string uri) : BaseProvider(uri)
{
    private readonly Faker faker = new();

    public UserRequest CreateUserMock()
    {
        var newUserRequest = new UserRequest
        {
            Company = new CompanyRequest
            {
                Cnpj = this.faker.Company.Cnpj(includeFormatSymbols: false),
                Name = this.faker.Company.CompanyName()
            },
            Email = this.faker.Internet.Email(),
            Name = this.faker.Person.FullName,
            Password = this.faker.Internet.Password()
        };

        return newUserRequest;
    }

    public async Task<UserResponse> CreateNewUserAsync(UserRequest userRequest)
    {
        return await this.PostAsync<UserResponse, UserRequest>("register", userRequest);
    }
}
