using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.User;

public class CreateNewUserHandlerTests : IDisposable
{
    public CreateNewUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.db = new DefaultContext(options,
            new TestCompanyContext());
        this.handler = new CreateNewUserHandler(
            this.db
        );
        this.faker = new Faker();

        this.adminRoleId = Guid.NewGuid();
        SeedAdminRole();
    }

    private void SeedAdminRole()
    {
        var adminRole = new RoleModel { Id = this.adminRoleId, Name = "Admin" };
        this.db.AuthRoles.Add(adminRole);
        this.db.SaveChanges();
    }

    private readonly Guid adminRoleId;

    public void Dispose()
    {
        this.db.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext db;
    private readonly CreateNewUserHandler handler;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenValidRequest_CreatesUserAndCompanySuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        var user = await this.db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email,
            user.Email);
        Assert.Equal(name,
            user.Name);
        Assert.NotEqual(password,
            user.Password);

        var company = await this.db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.Equal(companyName,
            company.Name);
        Assert.Equal(cnpj,
            company.Cnpj);
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var existingUser = new UserModel { Email = email, Name = "Existing User", Password = "password" };
        this.db.AuthUsers.Add(existingUser);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None)
        );
        Assert.Equal("This email already exists",
            ex.Message);
    }

    [Fact]
    public async Task Handle_WhenCompanyAlreadyExists_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var existingCompany = new CompanyModel { Cnpj = cnpj, Name = "Existing Company" };
        this.db.AuthCompanies.Add(existingCompany);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None)
        );
        Assert.Equal("Company with this CNPJ already exists.",
            ex.Message);
    }

    [Fact]
    public async Task Handle_WhenAdminRoleNotFound_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        var adminRole = this.db.AuthRoles.First();
        this.db.AuthRoles.Remove(adminRole);
        await this.db.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request,
                CancellationToken.None)
        );

        Assert.Equal("Admin role not found. Please ensure that the admin role exists in the database.",
            ex.Message);
    }

    [Fact]
    public async Task Handle_CreatesUserRoleLinkingUserCompanyAndRole()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        var userRole = await this.db.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        Assert.NotNull(userRole);
        Assert.Equal(this.adminRoleId,
            userRole.RoleId);
        Assert.NotEqual(Guid.Empty,
            userRole.CompanyId);
    }

    [Fact]
    public async Task Handle_ReturnsCorrectResponseData()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act
        var result = await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty,
            result.Id);
        Assert.Equal(name,
            result.Name);
        Assert.Equal(email,
            result.Email);
        Assert.NotEqual(Guid.Empty,
            result.Company.Id);
        Assert.Equal(companyName,
            result.Company.Name);
        Assert.Equal(cnpj,
            result.Company.Cnpj);
    }

    [Fact]
    public async Task Handle_PasswordIsHashedBeforeSaving()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act
        await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        var user = await this.db.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password,
            user.Password);
    }

    [Fact]
    public async Task Handle_CompanyIsActiveByDefault()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        var request = new CreateNewUserCommand(email,
            password,
            name,
            new CreateNewUserCompanyCommand(cnpj,
                companyName));

        // Act
        await this.handler.Handle(request,
            CancellationToken.None);

        // Assert
        var company = await this.db.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.True(company.IsActive);
    }
}
