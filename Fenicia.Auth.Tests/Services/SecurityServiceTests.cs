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
        this.sut = new SecurityService();
        this.faker = new Faker();
    }

    [Test]
    public void HashPasswordWithValidPasswordReturnsHashedPassword()
    {
        var password = this.faker.Internet.Password(12, false, string.Empty, "!@#$%^&*");

        var result = this.sut.HashPassword(password);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.EqualTo(password));
            Assert.That(result, Does.StartWith("$2a$12$"), "Hash should use BCrypt format with work factor 12");
        }
    }

    [Test]
    public void HashPasswordWithSamePasswordGeneratesDifferentHashes()
    {
        var password = this.faker.Internet.Password();
        var hashes = new HashSet<string>();

        for (var i = 0; i < 5; i++)
        {
            var result = this.sut.HashPassword(password);
            hashes.Add(result);
        }

        Assert.That(hashes, Has.Count.EqualTo(5), "Each hash should be unique even for the same password");
    }

    [Test]
    [TestCase("")]
    public void HashPasswordWithInvalidPasswordThrowsException(string invalidPassword)
    {
        Assert.Throws<ArgumentException>(() => this.sut.HashPassword(invalidPassword));
    }

    [Test]
    public void VerifyPasswordWithCorrectPasswordReturnsTrue()
    {
        var password = this.faker.Internet.Password();
        var hashedPassword = this.sut.HashPassword(password);

        var result = this.sut.VerifyPassword(password, hashedPassword);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
        }
    }

    [Test]
    public void VerifyPasswordWithIncorrectPasswordReturnsFalse()
    {
        var password = this.faker.Internet.Password();
        var wrongPassword = this.faker.Internet.Password();
        var hashedPassword = this.sut.HashPassword(password);

        var result = this.sut.VerifyPassword(wrongPassword, hashedPassword);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
        }
    }

    [Test]
    public void VerifyPasswordWithInvalidHashReturnsErrorResponse()
    {
        var password = this.faker.Internet.Password();
        var invalidHash = this.faker.Random.AlphaNumeric(60);

        var result = this.sut.VerifyPassword(password, invalidHash);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.False);
        }
    }

    [Test]
    public void VerifyPasswordWithMultiplePasswordsWorksConsistently()
    {
        var passwordCount = this.faker.Random.Int(3, 5);
        var passwords = this.faker.Make(passwordCount, () => this.faker.Internet.Password()).ToList();
        var hashedPasswords = passwords.Select(p => this.sut.HashPassword(p)).ToList();

        for (var i = 0; i < passwordCount; i++)
        {
            var correctResult = this.sut.VerifyPassword(passwords[i], hashedPasswords[i]);
            Assert.That(correctResult, Is.True, $"Password {i} should verify against its own hash");

            for (var j = 0; j < passwordCount; j++)
            {
                if (i == j)
                {
                    continue;
                }

                var incorrectResult = this.sut.VerifyPassword(passwords[i], hashedPasswords[j]);
                Assert.That(incorrectResult, Is.False, $"Password {i} should not verify against hash {j}");
            }
        }
    }

    [Test]
    public void HashPasswordWithVariousPasswordComplexitiesGeneratesValidHashes()
    {
        var testPasswords = new[]
        {
            this.faker.Internet.Password(8), this.faker.Internet.Password(16, true),
            this.faker.Internet.Password(32, true, "@#$%")
        };

        foreach (var password in testPasswords)
        {
            var result = this.sut.HashPassword(password);
            var verifyResult = this.sut.VerifyPassword(password, result);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("$2a$12$"));
                Assert.That(verifyResult, Is.True);
            }
        }
    }

    [Test]
    public void VerifyPasswordWithNullOrEmptyValuesReturnsErrorResponse()
    {
        var validPassword = this.faker.Internet.Password();
        var validHash = this.sut.HashPassword(validPassword);
        var testCases = new[]
        {
            (Password: string.Empty, Hash: validHash),
            (Password: null, Hash: validHash),
            (Password: validPassword, Hash: string.Empty),
            (Password: validPassword, Hash: null)
        };

        foreach (var (password, hash) in testCases)
        {
            var result = this.sut.VerifyPassword(password!, hash!);

            using (Assert.EnterMultipleScope())
            {
                Assert.That(result, Is.False);
            }
        }
    }
}
