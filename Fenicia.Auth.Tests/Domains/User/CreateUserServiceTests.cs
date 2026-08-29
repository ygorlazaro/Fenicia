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

public class CreateUserServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;

    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public CreateUserServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository);
        _userService = new UserService(_userRepository, userRoleService, roleService, companyService, new SecurityService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserSuccessfully()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        var result = await _userService.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.NotEqual(Guid.Empty, result.Id);

        var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var existingUser = new UserModel
        {
            Email = email,
            Password = new SecurityService().Hash(password),
            Name = name
        };

        await _userRepository.InsertAsync(existingUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateUserCommand(email, password, "Another " + name);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenValidRequestWithCompanies_CreatesUserWithCompaniesSuccessfully()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var company = new CompanyModel
        {
            Name = _faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        var role = new RoleModel { Name = "Admin" };

        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand> { new(company.Id, role.Id) };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var result = await _userService.CreateAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        var userRole = await _userRoleRepository.Query().FirstOrDefaultAsync(ur => ur.UserId == result.Id);

        Assert.NotNull(userRole);
        Assert.Equal(company.Id, userRole.CompanyId);
        Assert.Equal(role.Id, userRole.RoleId);
    }

    [Fact]
    public async Task Handle_WhenCompanyNotFound_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var role = new RoleModel { Name = "Admin" };
        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
        {
        new(Guid.NewGuid(), role.Id)
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenRoleNotFound_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var company = new CompanyModel
        {
            Name = _faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<CreateUserRoleCommand>
        {
        new(company.Id, Guid.NewGuid())
        };

        var request = new CreateUserCommand(email, password, name, companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task Handle_PasswordIsHashed_BeforeSaving()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;

        var request = new CreateUserCommand(email, password, name);

        await _userService.CreateAsync(request, CancellationToken.None);

        var user = _db.Set<UserModel>().Local.FirstOrDefault(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password);
        Assert.StartsWith("$2", user.Password);
    }
}
