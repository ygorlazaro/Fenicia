using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CreateUserHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;

    private readonly UserService userService;
    private readonly UserRepository userRepository;
    private readonly UserRoleRepository userRoleRepository;
    private readonly RoleRepository roleRepository;
    private readonly CompanyRepository companyRepository;

    public CreateUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userRepository = new UserRepository(db);
        userRoleRepository = new UserRoleRepository(db);
        roleRepository = new RoleRepository(db);
        companyRepository = new CompanyRepository(db);
        userService = new UserService(userRepository, userRoleRepository, roleRepository, companyRepository);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserSuccessfully()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        var result = await userService.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);

        var user = await userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ThrowsArgumentException()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var existingUser = new UserModel
        {
            Email = email,
            Password = SecurityService.Hash(password),
            Name = name
        };

        userRepository.InsertAsync(existingUser, CancellationToken.None).GetAwaiter().GetResult();
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateUserCommand(email, password, "Another " + name);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenValidRequestWithCompanies_CreatesUserWithCompaniesSuccessfully()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var company = new CompanyModel
        {
            Name = faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        var role = new RoleModel { Name = "Admin" };

        await companyRepository.InsertAsync(company, CancellationToken.None);
        await roleRepository.InsertAsync(role, CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand> { new(company.Id, role.Id) };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var result = await userService.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        var userRole = await userRoleRepository.Query().FirstOrDefaultAsync(ur => ur.UserId == result.Id);

        Assert.NotNull(userRole);
        Assert.Equal(company.Id, userRole.CompanyId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var role = new RoleModel { Name = "Admin" };
        await roleRepository.InsertAsync(role, CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
    { new(Guid.NewGuid(), role.Id)
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var company = new CompanyModel
        {
            Name = faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        await companyRepository.InsertAsync(company, CancellationToken.None);
        await db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
    { new(company.Id, Guid.NewGuid())
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_PasswordIsHashed_BeforeSaving()
    {
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        await userService.CreateAsync(request, CancellationToken.None);

        var user = db.Set<UserModel>().Local.FirstOrDefault(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password);
        Assert.StartsWith("$2", user.Password);
    }
}
