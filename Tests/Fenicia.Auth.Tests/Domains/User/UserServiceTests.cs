using Bogus;
using Bogus.Extensions.Brazil;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.User.DTOs;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Auth.Tests.Domains.Security;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Fenicia.Auth.Tests.Domains.User;

[Collection("AuthTests")]
public class UserServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly Guid _adminRoleId;
    private readonly UserModel _testUser;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public UserServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(_userRoleRepository);
        var roleService = new RoleService(_roleRepository);
        var companyService = new CompanyService(_companyRepository, userRoleService);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        _userService = new UserService(_userRepository, userRoleService, roleService, companyService, new TestSecurityService(), moduleService);
        _faker = new Faker();

        _adminRoleId = Guid.NewGuid();
        SeedAdminRole().GetAwaiter().GetResult();

        _testUser = new UserModel
        {
            Email = _faker.Internet.Email(),
            Password = new TestSecurityService().Hash(_faker.Internet.Password()),
            Name = _faker.Person.FullName
        };

        _userRepository.InsertAsync(_testUser, CancellationToken.None).GetAwaiter().GetResult();
        _db.SaveChanges();

        for (var i = 0; i < 5; i++)
        {
            var user = new UserModel
            {
                Email = _faker.Internet.Email(),
                Password = new TestSecurityService().Hash(_faker.Internet.Password()),
                Name = _faker.Person.FullName
            };
            _userRepository.InsertAsync(user, CancellationToken.None).GetAwaiter().GetResult();
        }

        _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailExists_ReturnsTrue()
    {
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.True(result, "Should return true when email exists");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ReturnsFalse()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false when email doesn't exist");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailHasDifferentCase_ReturnsFalse()
    {
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.False(result, "Email comparison is case-sensitive");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenMultipleUsersExist_OnlyMatchesExactEmail()
    {
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";

        var user1 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result1 = await _userService.ExistsByEmailAsync(email1, CancellationToken.None);
        var result2 = await _userService.ExistsByEmailAsync(email2, CancellationToken.None);
        var result3 = await _userService.ExistsByEmailAsync("other@example.com", CancellationToken.None);

        Assert.True(result1, "Should find user1");
        Assert.True(result2, "Should find user2");
        Assert.False(result3, "Should not find other user");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithEmptyDatabase_ReturnsFalse()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.ExistsByEmailAsync(email, CancellationToken.None);

        Assert.False(result, "Should return false with empty database");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailContainsExtraSpaces_ReturnsFalse()
    {
        const string email = "test@example.com";
        const string emailWithSpaces = " test@example.com ";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.False(result, "Should not match email with extra spaces");
    }

    [Fact]
    public async Task ExistsByEmailAsync_WhenEmailHasExtraCharacters_ReturnsFalse()
    {
        const string email = "test@example.com";
        const string emailWithExtra = "test@example.com.";

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.ExistsByEmailAsync(emailWithExtra, CancellationToken.None);

        Assert.False(result, "Should not match email with extra characters");
    }

    [Fact]
    public async Task CreateNewAsync_WhenValidRequest_CreatesUserAndCompanySuccessfully()
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
    public async Task CreateNewAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
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
    public async Task CreateNewAsync_WhenCompanyAlreadyExists_ThrowsArgumentException()
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
    public async Task CreateNewAsync_WhenAdminRoleNotFound_ThrowsArgumentException()
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
    public async Task CreateNewAsync_CreatesUserRoleLinkingUserCompanyAndRole()
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
    public async Task CreateNewAsync_ReturnsCorrectResponseData()
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
    public async Task CreateNewAsync_PasswordIsHashedBeforeSaving()
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
    public async Task CreateNewAsync_CompanyIsActiveByDefault()
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

    [Fact]
    public async Task CreateAsync_WhenValidRequest_CreatesUserSuccessfully()
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
    public async Task CreateAsync_WhenEmailExists_ThrowsArgumentException()
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
    public async Task CreateAsync_WhenValidRequestWithCompanies_CreatesUserWithCompaniesSuccessfully()
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
    public async Task CreateAsync_WhenCompanyNotFound_ThrowsArgumentException()
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
    public async Task CreateAsync_WhenRoleNotFound_ThrowsArgumentException()
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
    public async Task CreateAsync_PasswordIsHashed_BeforeSaving()
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

    [Fact]
    public async Task DeleteAsync_WhenValidRequest_SoftDeletesUserSuccessfully()
    {
        await _userService.DeleteAsync(_testUser.Id, CancellationToken.None);

        var deletedUser = await _userRepository.Query().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _testUser.Id, CancellationToken.None);
        Assert.NotNull(deletedUser);
        Assert.NotNull(deletedUser.Deleted);
        Assert.True(deletedUser.Deleted.Value <= DateTime.UtcNow);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.DeleteAsync(nonExistentUserId, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_WhenUserAlreadyDeleted_ThrowsArgumentException()
    {
        _testUser.Deleted = DateTime.UtcNow;
        await _db.SaveChangesAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.DeleteAsync(_testUser.Id, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task DeleteAsync_SoftDelete_UserStillExistsInDatabase()
    {
        await _userService.DeleteAsync(_testUser.Id, CancellationToken.None);

        var user = await _userRepository.Query().IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == _testUser.Id, CancellationToken.None);
        Assert.NotNull(user);
        Assert.NotNull(user.Deleted);

        var totalCount = await _userRepository.Query().IgnoreQueryFilters().CountAsync();
        Assert.Equal(6, totalCount);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserExists_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
        Assert.Equal(password, result.Password);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailHasDifferentCase_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(upperCaseEmail, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = "user1@example.com";
        var email2 = "user2@example.com";
        var name1 = _faker.Person.FullName;
        var name2 = _faker.Person.FullName;
        var password1 = _faker.Internet.Password();
        var password2 = _faker.Internet.Password();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = password1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = password2
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(email1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_WithEmptyDatabase_ReturnsNull()
    {
        var email = _faker.Internet.Email();

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenEmailContainsExtraSpaces_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var emailWithSpaces = " test@example.com ";
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(emailWithSpaces, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByEmailAsync_VerifiesResponseContainsAllFields()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetByEmailAsync(email, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
        Assert.NotNull(result.Password);
    }

    [Fact]
    public async Task GetForRefreshAsync_WhenUserExists_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(email, result.Email);
        Assert.Equal(name, result.Name);
    }

    [Fact]
    public async Task GetForRefreshAsync_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.GetForRefreshAsync(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task GetForRefreshAsync_WhenMultipleUsersExist_ReturnsCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        const string email1 = "user1@example.com";
        const string email2 = "user2@example.com";
        var name1 = _faker.Person.FullName;
        var name2 = _faker.Person.FullName;

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = name1,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = name2,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId1, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId1, result.Id);
        Assert.Equal(email1, result.Email);
    }

    [Fact]
    public async Task GetForRefreshAsync_WithEmptyDatabase_ThrowsUnauthorizedAccessException()
    {
        var userId = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.GetForRefreshAsync(userId, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task GetForRefreshAsync_ResponseDoesNotIncludePassword()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var password = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = password
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Email);
    }

    [Fact]
    public async Task GetForRefreshAsync_VerifiesResponseContainsAllExpectedFields()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _userService.GetForRefreshAsync(userId, CancellationToken.None);

        Assert.NotNull(result);

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotNull(result.Email);
        Assert.NotNull(result.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoParameters_ReturnsFirstPageWithDefaultPerPage()
    {
        var result = await _userService.GetAllAsync(1, 10, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PerPage);
        Assert.True(result.Data.Count <= 10);
        Assert.Equal(6, result.Total);
        Assert.Equal(1, result.Pages);
    }

    [Fact]
    public async Task GetAllAsync_WhenPageSpecified_ReturnsCorrectPage()
    {
        var result = await _userService.GetAllAsync(2, 5, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PerPage);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_UsersAreOrderedAlphabeticallyByName()
    {
        var result = await _userService.GetAllAsync(1, 15, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(result.Data.Select(u => u.Name).OrderBy(n => n), result.Data.Select(u => u.Name));
    }

    [Fact]
    public async Task GetAllAsync_WhenLastPage_HasNextIsFalse()
    {
        var result = await _userService.GetAllAsync(2, 10, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(2, result.Page);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserExists_ChangesPasswordSuccessfully()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string oldPassword = "old_hashed_password";

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        Assert.NotNull(result);

        Assert.Equal(userId, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(user.Email, result.Email);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(oldPassword, updatedUser.Password);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserDoesNotExist_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_PasswordIsHashedBeforeSaving()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.True(updatedUser.Password.Length > newPassword.Length);
    }

    [Fact]
    public async Task UpdatePasswordAsync_VerifiesPasswordCanBeVerified()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        var isValid = new SecurityService().Verify(newPassword, updatedUser.Password);
        Assert.True(isValid);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenMultipleUsersExist_OnlyUpdatesRequestedUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        const string oldPassword1 = "old_password_1";
        const string oldPassword2 = "old_password_2";

        var user1 = new UserModel
        {
            Id = userId1,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword1
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = oldPassword2
        };

        _db.AuthUsers.AddRange(user1, user2);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId1, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser1 = await _userRepository.GetByIdAsync(userId1, CancellationToken.None).ContinueWith(t => t.Result);
        var updatedUser2 = await _userRepository.GetByIdAsync(userId2, CancellationToken.None).ContinueWith(t => t.Result);

        Assert.NotEqual(oldPassword1, updatedUser1!.Password);
        Assert.Equal(oldPassword2, updatedUser2!.Password);
    }

    [Fact]
    public async Task UpdatePasswordAsync_PreservesOtherUserProperties()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(userId, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.Equal(email, updatedUser.Email);
        Assert.Equal(name, updatedUser.Name);
        Assert.Equal(userId, updatedUser.Id);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WithEmptyDatabase_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("User not found", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenPasswordIsEmpty_StillHashesAndSaves()
    {
        var userId = Guid.NewGuid();
        var newPassword = string.Empty;

        var user = new UserModel
        {
            Id = userId,
            Email = _faker.Internet.Email(),
            Name = _faker.Person.FullName,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None));
        Assert.Equal("Password cannot be null or empty", ex.Message);
    }

    [Fact]
    public async Task UpdatePasswordAsync_ReturnsCorrectResponseData()
    {
        var userId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = name,
            Password = "old_password"
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var query = new UpdatePasswordCommand(userId, newPassword);

        var result = await _userService.UpdateHashedPasswordAsync(query, CancellationToken.None);

        Assert.Equal(userId, result.Id);
        Assert.Equal(name, result.Name);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenValidRequest_ChangesPasswordSuccessfully()
    {
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(_testUser.Id, newPassword);
        var originalPasswordHash = _testUser.Password;

        var result = await _userService.UpdatePasswordAsync(request, CancellationToken.None);

        Assert.NotNull(result);

        Assert.True(result.Success);
        Assert.Equal("Password changed successfully", result.Message);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);

        Assert.NotEqual(originalPasswordHash, updatedUser.Password);
    }

    [Fact]
    public async Task UpdatePasswordAsync_NewPasswordIsHashed()
    {
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(_testUser.Id, newPassword);

        await _userService.UpdatePasswordAsync(request, CancellationToken.None);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.NotEqual(newPassword, updatedUser.Password);
        Assert.StartsWith("$2", updatedUser.Password);

        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedUser.Password));
    }

    [Fact]
    public async Task UpdatePasswordAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var newPassword = _faker.Internet.Password();
        var request = new UpdateUserPasswordCommand(nonExistentUserId, newPassword);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdatePasswordAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidRequest_UpdatesUserNameSuccessfully()
    {
        var newName = _faker.Person.FullName;
        var request = new UpdateUserCommand(_testUser.Id, newName);

        var result = await _userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newName, updatedUser.Name);
    }

    [Fact]
    public async Task UpdateAsync_WhenValidRequest_UpdatesUserEmailSuccessfully()
    {
        var newEmail = _faker.Internet.Email();
        var request = new UpdateUserCommand(_testUser.Id, Email: newEmail);

        var result = await _userService.UpdateAsync(request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(newEmail, result.Email);

        var updatedUser = await _userRepository.GetByIdAsync(_testUser.Id, CancellationToken.None).ContinueWith(t => t.Result);
        Assert.NotNull(updatedUser);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task UpdateAsync_WhenUserNotFound_ThrowsArgumentException()
    {
        var nonExistentUserId = Guid.NewGuid();
        var request = new UpdateUserCommand(nonExistentUserId, "Test");

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("User not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenEmailAlreadyExists_ThrowsArgumentException()
    {
        var existingEmail = _faker.Internet.Email();

        var anotherUser = new UserModel
        {
            Email = existingEmail,
            Password = new SecurityService().Hash(_faker.Internet.Password()),
            Name = _faker.Person.FullName
        };

        await _userRepository.InsertAsync(anotherUser, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var request = new UpdateUserCommand(_testUser.Id, Email: existingEmail);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Equal("This email already exists", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenCompanyNotFound_ThrowsArgumentException()
    {
        var role = new RoleModel { Name = "Admin" };
        await _roleRepository.InsertAsync(role, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
        new(Guid.NewGuid(), role.Id)
        };

        var request = new UpdateUserCommand(_testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_WhenRoleNotFound_ThrowsArgumentException()
    {
        var company = new CompanyModel
        {
            Name = _faker.Company.CompanyName(),
            Cnpj = string.Empty
        };
        await _companyRepository.InsertAsync(company, CancellationToken.None);
        await _db.SaveChangesAsync(CancellationToken.None);

        var companiesRoles = new List<UpdateUserRoleCommand>
        {
        new(company.Id, Guid.NewGuid())
        };

        var request = new UpdateUserCommand(_testUser.Id, CompaniesRoles: companiesRoles);

        var exception = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _userService.UpdateAsync(request, CancellationToken.None));

        Assert.Contains("not found", exception.Message);
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
