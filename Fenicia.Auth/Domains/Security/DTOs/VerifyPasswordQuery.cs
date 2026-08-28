namespace Fenicia.Auth.Domains.Security.DTOs;

public record VerifyPasswordQuery(string Password, string HashedPassword);
