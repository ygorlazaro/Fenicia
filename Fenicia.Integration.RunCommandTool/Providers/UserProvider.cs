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
                Cnpj = faker.Company.Cnpj(includeFormatSymbols: false),
                Name = faker.Company.CompanyName()
            },
            Email = faker.Internet.Email(),
            Name = faker.Person.FullName,
            Password = faker.Internet.Password()
        };

        return newUserRequest;
    }

    public async Task<UserResponse> CreateNewUserAsync(UserRequest userRequest)
    {
        return await PostAsync<UserResponse, UserRequest>("register", userRequest);
    }
}
