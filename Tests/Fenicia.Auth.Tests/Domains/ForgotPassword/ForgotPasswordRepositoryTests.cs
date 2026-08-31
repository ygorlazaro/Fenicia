using Bogus;

using Fenicia.Auth.Domains.ForgotPassword;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Domains.ForgotPassword;

public class ForgotPasswordRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ForgotPasswordRepository _repository;

    public ForgotPasswordRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _repository = new ForgotPasswordRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetActiveByUserIdAndCodeAsync_WhenActiveCodeExists_ReturnsForgotPassword()
    {
        var userId = Guid.NewGuid();
        var code = _faker.Random.AlphaNumeric(6);

        var forgotPassword = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetActiveByUserIdAndCodeAsync(userId, code, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(code, result.Code);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetActiveByUserIdAndCodeAsync_WhenCodeIsExpired_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var code = _faker.Random.AlphaNumeric(6);

        var forgotPassword = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddMinutes(-1)
        };

        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetActiveByUserIdAndCodeAsync(userId, code, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveByUserIdAndCodeAsync_WhenCodeIsInactive_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var code = _faker.Random.AlphaNumeric(6);

        var forgotPassword = new ForgotPasswordModel
        {
            Code = code,
            IsActive = false,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetActiveByUserIdAndCodeAsync(userId, code, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveByUserIdAndCodeAsync_WhenUserIdDoesNotMatch_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var code = _faker.Random.AlphaNumeric(6);

        var forgotPassword = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = otherUserId,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetActiveByUserIdAndCodeAsync(userId, code, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveByUserIdAndCodeAsync_WhenCodeDoesNotMatch_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        var code = _faker.Random.AlphaNumeric(6);
        var otherCode = _faker.Random.AlphaNumeric(6);

        var forgotPassword = new ForgotPasswordModel
        {
            Code = code,
            IsActive = true,
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddHours(1)
        };

        _db.AuthForgottenPasswords.Add(forgotPassword);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetActiveByUserIdAndCodeAsync(userId, otherCode, CancellationToken.None);

        Assert.Null(result);
    }
}
