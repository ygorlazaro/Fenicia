using Bogus;
using Bogus.Extensions.Brazil;
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

public class CreateNewUserServiceTests : IDisposable
{
    private readonly Guid _adminRoleId;

    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public CreateNewUserServiceTests()
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

        _adminRoleId = Guid.NewGuid();

        SeedAdminRole().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserAndCompanySuccessfully()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await _userService.CreateNewAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.NotEqual(password, user.Password);

        var company = await _companyRepository.Query().FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.Equal(companyName, company.Name);
        Assert.Equal(cnpj, company.Cnpj);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var existingUser = new UserModel
        {
            Email = email,
            Name = "Existing User",
            Password = "password"
        };
        await _userRepository.InsertAsync(existingUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateNewAsync(request, CancellationToken.None));
        Assert.Equal("This email already exists", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var existingCompany = new CompanyModel
        {
            Cnpj = cnpj,
            Name = "Existing Company"
        };
        await _companyRepository.InsertAsync(existingCompany, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateNewAsync(request, CancellationToken.None));
        Assert.Equal("Company with this CNPJ already exists.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNotFound_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var adminRole = _db.AuthRoles.First();
        _db.AuthRoles.Remove(adminRole);
        await _db.SaveChangesAsync(CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.CreateNewAsync(request, CancellationToken.None));

        Assert.Equal("Admin role not found. Please ensure that the admin role exists in the database.", ex.Message);
    }

    [Fact]
    public async Task Handle_CreatesUserRoleLinkingUserCompanyAndRole()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await _userService.CreateNewAsync(request, CancellationToken.None);

        var userRole = await _userRoleRepository.Query().FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        Assert.NotNull(userRole);
        Assert.Equal(_adminRoleId, userRole.RoleId);
        Assert.NotEqual(Guid.Empty, userRole.CompanyId);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponseData()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await _userService.CreateNewAsync(request, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);
        Assert.NotEqual(Guid.Empty, result.Company.Id);
        Assert.Equal(companyName, result.Company.Name);
        Assert.Equal(cnpj, result.Company.Cnpj);
    }

    [Fact]
    public async Task Handle_PasswordIsHashedBeforeSaving()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        await _userService.CreateNewAsync(request, CancellationToken.None);

        var user = await _userRepository.Query().FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password);
    }

    [Fact]
    public async Task Handle_CompanyIsActiveByDefault()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var name = _faker.Person.FullName;
        var cnpj = _faker.Company.Cnpj();
        var companyName = _faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        await _userService.CreateNewAsync(request, CancellationToken.None);

        var company = await _companyRepository.Query().FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.True(company.IsActive);
    }

    private async Task SeedAdminRole()
    {
        var adminRole = new RoleModel
        {
            Id = _adminRoleId,
            Name = "Admin"
        };
        await _roleRepository.InsertAsync(adminRole, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);
    }
}
