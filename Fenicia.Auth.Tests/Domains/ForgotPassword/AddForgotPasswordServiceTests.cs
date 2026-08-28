using Bogus;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Auth.Domains.ForgotPassword.DTOs;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class AddForgotPasswordServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ForgotPasswordService _service;
    private readonly UserRepository _userRepository;
    private readonly UserRoleRepository _userRoleRepository;
    private readonly RoleRepository _roleRepository;
    private readonly CompanyRepository _companyRepository;

    public AddForgotPasswordServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _userRepository = new UserRepository(_db);
        _userRoleRepository = new UserRoleRepository(_db);
        _roleRepository = new RoleRepository(_db);
        _companyRepository = new CompanyRepository(_db);
        var userService = new UserService(_userRepository, _userRoleRepository, _roleRepository, _companyRepository);
        _service = new ForgotPasswordService(_db, userService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenEmailExists_CreatesForgotPasswordCodeSuccessfully()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.SaveChanges();

        var command = new AddForgotPasswordCommand(email);

        await _service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(6, forgotPassword.Code.Length);
        Assert.True(forgotPassword.IsActive);
        Assert.Equal(userId, forgotPassword.UserId);
        Assert.True(forgotPassword.ExpirationDate > DateTime.UtcNow);
        Assert.Null(forgotPassword.IpAddress);
        Assert.Null(forgotPassword.UserAgent);
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_ThrowsItemNotExistsException()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.SaveChanges();

        var command = new AddForgotPasswordCommand(upperCaseEmail);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenMultipleUsersExist_CreatesCodeForCorrectUser()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        _db.SaveChanges();

        var command = new AddForgotPasswordCommand(email1);

        await _service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId1);
        Assert.NotNull(forgotPassword);
        Assert.Equal(userId1, forgotPassword.UserId);
        Assert.Equal(6, forgotPassword.Code.Length);

        var forgotPasswordForUser2 = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId2);
        Assert.Null(forgotPasswordForUser2);
    }

    [Fact]
    public async Task Handle_WhenCalledMultipleTimesForSameUser_CreatesMultipleCodes()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.SaveChanges();

        var command = new AddForgotPasswordCommand(email);

        await _service.AddAsync(command, CancellationToken.None);
        await _service.AddAsync(command, CancellationToken.None);

        var codes = await _db.AuthForgottenPasswords.Where(fp => fp.UserId == userId).ToListAsync();
        Assert.Equal(2, codes.Count);
        Assert.True(codes.All(c => c.IsActive));
        Assert.True(codes.All(c => c.Code.Length == 6));
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ThrowsItemNotExistsException()
    {
        var email = _faker.Internet.Email();
        var command = new AddForgotPasswordCommand(email);

        var ex = await Assert.ThrowsAsync<ItemNotExistsException>(async () => await _service.AddAsync(command, CancellationToken.None));
        Assert.Equal("User with given email does not exist.", ex.Message);
    }

    [Fact]
    public async Task Handle_VerifiesCodeIsUnique()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var email1 = _faker.Internet.Email();
        var email2 = _faker.Internet.Email();

        var user1 = new UserModel
        {
            Id = userId1,
            Email = email1,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        var user2 = new UserModel
        {
            Id = userId2,
            Email = email2,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        _db.AuthUsers.AddRange(user1, user2);
        _db.SaveChanges();

        var command1 = new AddForgotPasswordCommand(email1);
        var command2 = new AddForgotPasswordCommand(email2);

        await _service.AddAsync(command1, CancellationToken.None);
        await _service.AddAsync(command2, CancellationToken.None);

        var codes = await _db.AuthForgottenPasswords.ToListAsync();
        var distinctCodes = codes.Select(c => c.Code).Distinct().ToList();
        Assert.Equal(2, distinctCodes.Count);
    }

    [Fact]
    public async Task Handle_WhenIpAddressAndUserAgentProvided_StoresThemCorrectly()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0 (Test Browser)";

        var user = new UserModel
        {
            Id = userId,
            Email = email,
            Name = _faker.Person.FullName,
            Password = _faker.Internet.Password()
        };

        await _userRepository.InsertAsync(user, CancellationToken.None);
        _db.SaveChanges();

        var command = new AddForgotPasswordCommand(email, ipAddress, userAgent);

        await _service.AddAsync(command, CancellationToken.None);

        var forgotPassword = await _db.AuthForgottenPasswords.FirstOrDefaultAsync(fp => fp.UserId == userId);
        Assert.NotNull(forgotPassword);
        Assert.Equal(ipAddress, forgotPassword.IpAddress);
        Assert.Equal(userAgent, forgotPassword.UserAgent);
    }
}
