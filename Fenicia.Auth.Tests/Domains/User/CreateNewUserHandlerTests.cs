using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CreateNewUserHandlerTests : IDisposable
{
    private readonly Guid adminRoleId;

    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly UserService userService;

    public CreateNewUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        userService = new UserService(db);
        faker = new Faker();

        adminRoleId = Guid.NewGuid();
        SeedAdminRole();
    }

    public void Dispose()
    {
        db.Dispose();

        GC.SuppressFinalize(this);
    }

    private void SeedAdminRole()
    {
        var adminRole = new RoleModel
        {
            Id = adminRoleId,
            Name = "Admin"
        };
        db.AuthRoles.Add(adminRole);
        db.SaveChanges();
    }

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserAndCompanySuccessfully()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await userService.CreateNewAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.NotEqual(password, user.Password);

        var company = await db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.Equal(companyName, company.Name);
        Assert.Equal(cnpj, company.Cnpj);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var existingUser = new UserModel
        {
            Email = email,
            Name = "Existing User",
            Password = "password"
        };
        db.AuthUsers.Add(existingUser);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateNewAsync(request, CancellationToken.None));
        Assert.Equal("This email already exists", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var existingCompany = new CompanyModel
        {
            Cnpj = cnpj,
            Name = "Existing Company"
        };
        db.AuthCompanies.Add(existingCompany);
        await db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateNewAsync(request, CancellationToken.None));
        Assert.Equal("Company with this CNPJ already exists.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNotFound_ThrowsArgumentException()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var adminRole = db.AuthRoles.First();
        db.AuthRoles.Remove(adminRole);
        await db.SaveChangesAsync(CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await userService.CreateNewAsync(request, CancellationToken.None));

        Assert.Equal("Admin role not found. Please ensure that the admin role exists in the database.", ex.Message);
    }

    [Fact]
    public async Task Handle_CreatesUserRoleLinkingUserCompanyAndRole()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await userService.CreateNewAsync(request, CancellationToken.None);

        var userRole = await db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        Assert.NotNull(userRole);
        Assert.Equal(adminRoleId, userRole.RoleId);
        Assert.NotEqual(Guid.Empty, userRole.CompanyId);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponseData()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        var result = await userService.CreateNewAsync(request, CancellationToken.None);

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

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        await userService.CreateNewAsync(request, CancellationToken.None);

        var user = await db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password);
    }

    [Fact]
    public async Task Handle_CompanyIsActiveByDefault()
    {

        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var name = faker.Person.FullName;
        var cnpj = faker.Company.Cnpj();
        var companyName = faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email, password, name, new CreateNewUserCompanyCommand(cnpj, companyName));

        await userService.CreateNewAsync(request, CancellationToken.None);

        var company = await db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.True(company.IsActive);
    }
}
