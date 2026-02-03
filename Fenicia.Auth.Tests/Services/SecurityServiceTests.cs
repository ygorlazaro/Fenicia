using System.Net;

using Bogus;

using Fenicia.Auth.Domains.Security;


namespace Fenicia.Auth.Tests.Services;

public class SecurityServiceTests
{
    private Faker faker;
    private SecurityService sut;

    [SetUp]
    public void Setup()
    {
        sut = new SecurityService();
        faker = new Faker();
    }

    [Test]
    public void HashPasswordWithValidPasswordReturnsHashedPassword()
    {
        // Arrange
        var password = faker.Internet.Password(length: 12, memorable: false, string.Empty, "!@#$%^&*");

        // Act
        var result = sut.HashPassword(password);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Not.EqualTo(password));
            Assert.That(result.Data!, Does.StartWith("$2a$12$"), "Hash should use BCrypt format with work factor 12");
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public void HashPasswordWithSamePasswordGeneratesDifferentHashes()
    {
        // Arrange
        var password = faker.Internet.Password();
        var hashes = new HashSet<string>();

        // Act
        for (var i = 0; i < 5; i++)
        {
            var result = sut.HashPassword(password);
            hashes.Add(result.Data!);
        }

        // Assert
        Assert.That(hashes, Has.Count.EqualTo(expected: 5), "Each hash should be unique even for the same password");
    }

    [Test]
    [TestCase("")]
    public void HashPasswordWithInvalidPasswordThrowsException(string invalidPassword)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => sut.HashPassword(invalidPassword));
    }

    [Test]
    public void VerifyPasswordWithCorrectPasswordReturnsTrue()
    {
        // Arrange
        var password = faker.Internet.Password();
        var hashedPassword = sut.HashPassword(password).Data;

        // Act
        var result = sut.VerifyPassword(password, hashedPassword!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.True);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public void VerifyPasswordWithIncorrectPasswordReturnsFalse()
    {
        // Arrange
        var password = faker.Internet.Password();
        var wrongPassword = faker.Internet.Password();
        var hashedPassword = sut.HashPassword(password).Data;

        // Act
        var result = sut.VerifyPassword(wrongPassword, hashedPassword!);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.OK));
        }
    }

    [Test]
    public void VerifyPasswordWithInvalidHashReturnsErrorResponse()
    {
        // Arrange
        var password = faker.Internet.Password();
        var invalidHash = faker.Random.AlphaNumeric(length: 60); // Invalid BCrypt hash format

        // Act
        var result = sut.VerifyPassword(password, invalidHash);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Data, Is.False);
            Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
        }
    }

    [Test]
    public void VerifyPasswordWithMultiplePasswordsWorksConsistently()
    {
        // Arrange
        var passwordCount = faker.Random.Int(min: 3, max: 5);
        var passwords = faker.Make(passwordCount, () => faker.Internet.Password()).ToList();
        var hashedPasswords = passwords.Select(p => sut.HashPassword(p).Data).ToList();

        // Act & Assert
        for (var i = 0; i < passwordCount; i++)
        {
            // Verify correct password
            var correctResult = sut.VerifyPassword(passwords[i], hashedPasswords[i]!);
            Assert.That(correctResult.Data, Is.True, $"Password {i} should verify against its own hash");

            // Verify against other passwords' hashes
            for (var j = 0; j < passwordCount; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var incorrectResult = sut.VerifyPassword(passwords[i], hashedPasswords[j]!);
                Assert.That(incorrectResult.Data, Is.False, $"Password {i} should not verify against hash {j}");
            }
        }
    }

    [Test]
    public void HashPasswordWithVariousPasswordComplexitiesGeneratesValidHashes()
    {
        // Arrange
        var testPasswords = new[]
                            {
                                faker.Internet.Password(length: 8), // Simple password
                                faker.Internet.Password(length: 16, memorable: true), // Complex password
                                faker.Internet.Password(length: 32, memorable: true, "@#$%") // Very complex password
                            };

        foreach (var password in testPasswords)
        {
            // Act
            var result = sut.HashPassword(password);
            var verifyResult = sut.VerifyPassword(password, result.Data!);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data, Is.Not.Null);
                Assert.That(result.Data, Does.StartWith("$2a$12$"));
                Assert.That(verifyResult.Data, Is.True);
            }
        }
    }

    [Test]
    public void VerifyPasswordWithNullOrEmptyValuesReturnsErrorResponse()
    {
        // Arrange
        var validPassword = faker.Internet.Password();
        var validHash = sut.HashPassword(validPassword).Data;
        var testCases = new[]
                        {
                            (Password: string.Empty, Hash: validHash),
                            (Password: null, Hash: validHash),
                            (Password: validPassword, Hash: string.Empty),
                            (Password: validPassword, Hash: null)
                        };

        foreach (var (password, hash) in testCases)
        {
            // Act
            var result = sut.VerifyPassword(password!, hash!);

            // Assert
            using (Assert.EnterMultipleScope())
            {
                Assert.That(result.Data, Is.False);
                Assert.That(result.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
            }
        }
    }
}
