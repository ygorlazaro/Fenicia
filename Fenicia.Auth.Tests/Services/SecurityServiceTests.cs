namespace Fenicia.Auth.Tests.Services;

using System.Net;

using Bogus;

using Domains.Security.Logic;

using Microsoft.Extensions.Logging;

using Moq;

public class SecurityServiceTests
{
    private Faker _faker;
    private SecurityService _sut;

    [SetUp]
    public void Setup()
    {
        var mockLogger = new Mock<ILogger<SecurityService>>().Object;
        _sut = new SecurityService(mockLogger);
        _faker = new Faker();
    }

    [Test]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = _faker.Internet.Password(length: 12, memorable: false, regexPattern: "", prefix: "!@#$%^&*");

        // Act
        var result = _sut.HashPassword(password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Not.EqualTo(password));
            Assert.That(result.Data!, Does.StartWith(expected: "$2a$12$"), message: "Hash should use BCrypt format with work factor 12");
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void HashPassword_WithSamePassword_GeneratesDifferentHashes()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var hashes = new HashSet<string>();

        // Act
        for (var i = 0; i < 5; i++)
        {
            var result = _sut.HashPassword(password);
            hashes.Add(result.Data!);
        }

        // Assert
        Assert.That(hashes, Has.Count.EqualTo(expected: 5), message: "Each hash should be unique even for the same password");
    }

    [Test]
    [TestCase(arg: "")]
    public void HashPassword_WithInvalidPassword_ThrowsException(string invalidPassword)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _sut.HashPassword(invalidPassword));
    }

    [Test]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var hashedPassword = _sut.HashPassword(password).Data;

        // Act
        var result = _sut.VerifyPassword(password, hashedPassword!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.True);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var wrongPassword = _faker.Internet.Password();
        var hashedPassword = _sut.HashPassword(password).Data;

        // Act
        var result = _sut.VerifyPassword(wrongPassword, hashedPassword!);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void VerifyPassword_WithInvalidHash_ReturnsErrorResponse()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var invalidHash = _faker.Random.AlphaNumeric(length: 60); // Invalid BCrypt hash format

        // Act
        var result = _sut.VerifyPassword(password, invalidHash);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
        });
    }

    [Test]
    public void VerifyPassword_WithMultiplePasswords_WorksConsistently()
    {
        // Arrange
        var passwordCount = _faker.Random.Int(min: 3, max: 5);
        var passwords = _faker.Make(passwordCount, () => _faker.Internet.Password()).ToList();
        var hashedPasswords = passwords.Select(p => _sut.HashPassword(p).Data).ToList();

        // Act & Assert
        for (var i = 0; i < passwordCount; i++)
        {
            // Verify correct password
            var correctResult = _sut.VerifyPassword(passwords[i], hashedPasswords[i]!);
            Assert.That(correctResult.Data, Is.True, $"Password {i} should verify against its own hash");

            // Verify against other passwords' hashes
            for (var j = 0; j < passwordCount; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var incorrectResult = _sut.VerifyPassword(passwords[i], hashedPasswords[j]!);
                Assert.That(incorrectResult.Data, Is.False, $"Password {i} should not verify against hash {j}");
            }
        }
    }

    [Test]
    public void HashPassword_WithVariousPasswordComplexities_GeneratesValidHashes()
    {
        // Arrange
        var testPasswords = new[]
                            {
                                _faker.Internet.Password(length: 8), // Simple password
                                _faker.Internet.Password(length: 16, memorable: true), // Complex password
                                _faker.Internet.Password(length: 32, memorable: true, regexPattern: "@#$%") // Very complex password
                            };

        foreach (var password in testPasswords)
        {
            // Act
            var result = _sut.HashPassword(password);
            var verifyResult = _sut.VerifyPassword(password, result.Data!);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data, Does.StartWith(expected: "$2a$12$"));
                Assert.That(verifyResult.Data, Is.True);
            });
        }
    }

    [Test]
    public void VerifyPassword_WithNullOrEmptyValues_ReturnsErrorResponse()
    {
        // Arrange
        var validPassword = _faker.Internet.Password();
        var validHash = _sut.HashPassword(validPassword).Data;
        var testCases = new[]
                        {
                            (Password: "", Hash: validHash),
                            (Password: null, Hash: validHash),
                            (Password: validPassword, Hash: ""),
                            (Password: validPassword, Hash: null)
                        };

        foreach (var (password, hash) in testCases)
        {
            // Act
            var result = _sut.VerifyPassword(password!, hash!);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Data, Is.False);
                Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }
    }
}
