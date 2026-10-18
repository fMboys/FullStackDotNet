using SafeVault.Web.Helpers;

namespace UserRegistration.Tests;

[TestFixture]
public class AuthenticationTests
{
    [Test]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        var password = "Admin123!";

        var hash =
            PasswordService.HashPassword(password);

        Assert.That(
            PasswordService.VerifyPassword(
                password,
                hash),
            Is.True);
    }

    [Test]
    public void VerifyPassword_WithWrongPassword_ReturnsFalse()
    {
        var hash =
            PasswordService.HashPassword("Admin123!");

        Assert.That(
            PasswordService.VerifyPassword(
                "WrongPassword",
                hash),
            Is.False);
    }
}