using AwesomeAssertions;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Tests;
using Fenicia.Module.SocialNetwork.Domains.Profile;
using Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Tests.Domains.Profile;

public class ProfileServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly ProfileService _service;

    public ProfileServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new ProfileRepository(_db);
        _service = new ProfileService(repository);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileExists_ReturnsProfile()
    {
        var profile = new ProfileModel
        {
            UserId = Guid.NewGuid(),
            Bio = _faker.Lorem.Sentence()
        };
        _db.SocialNetworkProfiles.Add(profile);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetProfileByIdQuery(profile.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(profile.Id);
        result.UserId.Should().Be(profile.UserId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProfileDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetProfileByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenProfileExists_UpdatesProfile()
    {
        var profile = new ProfileModel
        {
            UserId = Guid.NewGuid(),
            Bio = _faker.Lorem.Sentence()
        };
        _db.SocialNetworkProfiles.Add(profile);
        await _db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProfileCommand(profile.Id, "Updated Bio", "https://example.com/image.jpg", "https://example.com", "New York", "1234567890", new DateTime(1990, 1, 1));

        var result = await _service.UpdateAsync(command, profile.UserId, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Bio.Should().Be("Updated Bio");
        result.ImageUrl.Should().Be("https://example.com/image.jpg");
    }

    [Fact]
    public async Task UpdateAsync_WhenProfileDoesNotExist_ReturnsNull()
    {
        var command = new UpdateProfileCommand(Guid.NewGuid(), "Updated Bio", null, null, null, null, null);

        var result = await _service.UpdateAsync(command, Guid.NewGuid(), CancellationToken.None);

        result.Should().BeNull();
    }
}
