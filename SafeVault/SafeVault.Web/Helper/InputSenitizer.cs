using System.Text.RegularExpressions;
using System.Net;

namespace SafeVault.Web.Helpers;

public static class InputSanitizer
{
    public static string Sanitize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Trim whitespace
        input = input.Trim();

        // Remove script tags
        input = Regex.Replace(
            input,
            @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
            string.Empty,
            RegexOptions.IgnoreCase);

        // Remove common SQL injection characters
        input = Regex.Replace(
            input,
            @"[';""\\]|(--)",
            string.Empty);

        // Encode HTML special characters
        input = WebUtility.HtmlEncode(input);

        return input;
    }

    public static bool ContainsMaliciousContent(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        string[] dangerousPatterns =
        {
            "<script",
            "javascript:",
            "onerror=",
            "onload=",
            "drop table",
            "delete from",
            "insert into",
            "xp_cmdshell",
            "--",
            "union select"
        };

        return dangerousPatterns.Any(pattern =>
            input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }
}