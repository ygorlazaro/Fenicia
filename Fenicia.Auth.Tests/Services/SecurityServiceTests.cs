using System.Net;
using Bogus;

using Fenicia.Auth.Domains.Security;

namespace Fenicia.Auth.Tests.Services;

public class SecurityServiceTests
{
    private SecurityService _sut;
    private Faker _faker;

    [SetUp]
    public void Setup()
    {
        _sut = new SecurityService();
        _faker = new Faker();
    }

    [Test]
    public void HashPassword_WithValidPassword_ReturnsHashedPassword()
    {
        // Arrange
        var password = _faker.Internet.Password(12, false, "", "!@#$%^&*");

        // Act
        var result = _sut.HashPassword(password);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Not.EqualTo(password));
            Assert.That(
                result.Data.StartsWith("$2a$12$"),
                Is.True,
                "Hash should use BCrypt format with work factor 12"
            );
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void HashPassword_WithSamePassword_GeneratesDifferentHashes()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var hashes = new HashSet<string>();

        // Act
        for (int i = 0; i < 5; i++)
        {
            var result = _sut.HashPassword(password);
            hashes.Add(result.Data);
        }

        // Assert
        Assert.That(
            hashes.Count,
            Is.EqualTo(5),
            "Each hash should be unique even for the same password"
        );
    }

    [Test]
    [TestCase("")]
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
        var result = _sut.VerifyPassword(password, hashedPassword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.True);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
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
        var result = _sut.VerifyPassword(wrongPassword, hashedPassword);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        });
    }

    [Test]
    public void VerifyPassword_WithInvalidHash_ReturnsErrorResponse()
    {
        // Arrange
        var password = _faker.Internet.Password();
        var invalidHash = _faker.Random.AlphaNumeric(60); // Invalid BCrypt hash format

        // Act
        var result = _sut.VerifyPassword(password, invalidHash);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
        });
    }

    [Test]
    public void VerifyPassword_WithMultiplePasswords_WorksConsistently()
    {
        // Arrange
        var passwordCount = _faker.Random.Int(5, 10);
        var passwords = _faker.Make(passwordCount, () => _faker.Internet.Password()).ToList();
        var hashedPasswords = passwords.Select(p => _sut.HashPassword(p).Data).ToList();

        // Act & Assert
        for (int i = 0; i < passwordCount; i++)
        {
            // Verify correct password
            var correctResult = _sut.VerifyPassword(passwords[i], hashedPasswords[i]);
            Assert.That(
                correctResult.Data,
                Is.True,
                $"Password {i} should verify against its own hash"
            );

            // Verify against other passwords' hashes
            for (int j = 0; j < passwordCount; j++)
            {
                if (i == j)
                    continue;
                var incorrectResult = _sut.VerifyPassword(passwords[i], hashedPasswords[j]);
                Assert.That(
                    incorrectResult.Data,
                    Is.False,
                    $"Password {i} should not verify against hash {j}"
                );
            }
        }
    }

    [Test]
    public void HashPassword_WithVariousPasswordComplexities_GeneratesValidHashes()
    {
        // Arrange
        var testPasswords = new[]
        {
            _faker.Internet.Password(8, false), // Simple password
            _faker.Internet.Password(16, true), // Complex password
            _faker.Internet.Password(32, true, "@#$%"), // Very complex password
        };

        foreach (var password in testPasswords)
        {
            // Act
            var result = _sut.HashPassword(password);
            var verifyResult = _sut.VerifyPassword(password, result.Data);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data.StartsWith("$2a$12$"), Is.True);
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
            (Password: validPassword, Hash: null),
        };

        foreach (var (password, hash) in testCases)
        {
            // Act
            var result = _sut.VerifyPassword(password, hash);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Data, Is.False);
                Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
            });
        }
    }
}
