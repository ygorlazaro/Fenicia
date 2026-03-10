using Bogus;
using Bogus.Extensions.Brazil;

using Fenicia.Auth.Domains.Company.CheckCompanyExists;
using Fenicia.Auth.Domains.Role.GetAdminRole;
using Fenicia.Auth.Domains.Security.HashPassword;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.CreateNewUser;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;

using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

public class CreateNewUserHandlerTests : IDisposable
{
    public CreateNewUserHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        this.checkUserExistsHandlerMock = new Mock<CheckUserExistsHandler>(this.context);
        this.checkCompanyExistsHandlerMock = new Mock<CheckCompanyExistsHandler>(this.context);
        var hashPasswordHandler = new HashPasswordHandler();
        this.getAdminRoleHandlerMock = new Mock<GetAdminRoleHandler>(this.context);
        this.handler = new CreateNewUserHandler(
            this.context,
            this.checkUserExistsHandlerMock.Object,
            this.checkCompanyExistsHandlerMock.Object,
            hashPasswordHandler,
            this.getAdminRoleHandlerMock.Object
        );
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly CreateNewUserHandler handler;
    private readonly Mock<CheckUserExistsHandler> checkUserExistsHandlerMock;
    private readonly Mock<CheckCompanyExistsHandler> checkCompanyExistsHandlerMock;
    private readonly Mock<GetAdminRoleHandler> getAdminRoleHandlerMock;
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);

        var user = await this.context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.NotEqual(password, user.Password);

        var company = await this.context.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.Equal(companyName, company.Name);
        Assert.Equal(cnpj, company.Cnpj);
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.Equal("This email already exists", ex.Message);
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.Equal("Company with this CNPJ already exists.", ex.Message);
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetAdminRoleResponse?)null);

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(request, CancellationToken.None)
        );
        Assert.Equal("Admin role not found. Please ensure that the admin role exists in the database.", ex.Message);
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
        var adminRoleId = Guid.NewGuid();

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(adminRoleId, "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var userRole = await this.context.AuthUserRoles.FirstOrDefaultAsync(ur => ur.UserId == result.Id);
        Assert.NotNull(userRole);
        Assert.Equal(adminRoleId, userRole.RoleId);
        Assert.NotEqual(Guid.Empty, userRole.CompanyId);
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act
        var result = await this.handler.Handle(request, CancellationToken.None);

        // Assert
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
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var name = this.faker.Person.FullName;
        var cnpj = this.faker.Company.Cnpj();
        var companyName = this.faker.Company.CompanyName();

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var user = await this.context.AuthUsers.FirstOrDefaultAsync(u => u.Email == email);
        Assert.NotNull(user);
        Assert.NotEqual(password, user.Password);
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

        this.checkUserExistsHandlerMock
            .Setup(x => x.Handle(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.checkCompanyExistsHandlerMock
            .Setup(x => x.Handle(It.IsAny<CheckCompanyExistsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        this.getAdminRoleHandlerMock
            .Setup(x => x.Handle(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAdminRoleResponse(Guid.NewGuid(), "Admin"));

        var request = new CreateNewUserQuery(email, password, name,
            new CreateNewUserCompanyQuery(cnpj, companyName));

        // Act
        await this.handler.Handle(request, CancellationToken.None);

        // Assert
        var company = await this.context.AuthCompanies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        Assert.NotNull(company);
        Assert.True(company.IsActive);
    }
}
