using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Integration.RunCommandTool.Providers;

public class UserProvider : BaseProvider
{
    private readonly Faker _faker = new();

    public UserProvider(string baseUrl) : base(baseUrl)
    {
    }

    public UserRequest CreateUserMock()
    {
        var newUserRequest = new UserRequest()
        {
            Company = new CompanyRequest
            {
                Cnpj = _faker.Company.Cnpj(includeFormatSymbols: false),
                Name = _faker.Company.CompanyName()
            },
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };


        return newUserRequest;
    }

    public async Task<UserResponse> CreateNewUserAsync(UserRequest userRequest)
    {
        return await base.PostAsync<UserResponse, UserRequest>("signup", userRequest);
    }
}
