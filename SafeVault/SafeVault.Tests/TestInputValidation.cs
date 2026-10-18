using NUnit.Framework;
using SafeVault.Web.Helpers;

namespace SafeVault.Tests;

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void Should_Remove_Script_Tags()
    {
        var input = "<script>alert('xss')</script>John";

        var result = InputSanitizer.Sanitize(input);

        Assert.That(result, Does.Not.Contain("<script"));
    }

    [Test]
    public void Should_Detect_Xss_Attack()
    {
        var input = "<script>alert('xss')</script>";

        Assert.That(
            InputSanitizer.ContainsMaliciousContent(input),
            Is.True);
    }

    [Test]
    public void Should_Detect_Sql_Injection()
    {
        var input = "' OR 1=1 --";

        Assert.That(
            InputSanitizer.ContainsMaliciousContent(input),
            Is.True);
    }

    [Test]
    public void Should_Accept_Valid_Input()
    {
        var input = "JohnDoe123";

        Assert.That(
            InputSanitizer.ContainsMaliciousContent(input),
            Is.False);
    }
}
