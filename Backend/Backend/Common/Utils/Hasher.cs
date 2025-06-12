using Microsoft.AspNetCore.Identity;

namespace Backend.Common.Utils;

public static class Hasher
{
    private static readonly PasswordHasher<object> _hasher = new();

    public static string HashPassword(string password)
    {
        return _hasher.HashPassword(null, password);
    }

    public static PasswordVerificationResult VerifyHashedPassword(object user, string hashedPassword, string providedPassword)
    {
        return _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
    }

}
