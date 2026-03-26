using System.Text.RegularExpressions;

namespace InternshipDB.Helpers;

public static class CompanyTextHelper
{
    private static readonly Regex TimeRangeRegex = new(@"\b\d{1,2}([:.]\d{2})?\s*(am|pm)?\s*(?:-|–|to|till)\s*\d{1,2}([:.]\d{2})?\s*(am|pm)?\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex DayRangeRegex = new(@"\b(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)\s*(?:-|–|to)\s*(?:Mon|Tue|Wed|Thu|Fri|Sat|Sun|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex HoursRegex = new(@"\b\d+\s*(hours?|hrs?)\b(?:\s*(a|per)\s*week)?", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex DaysPerWeekRegex = new(@"\b\d+\s*days?\s*a\s*week\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex RosterRegex = new(@"\bRoster Based\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string GetInternshipPeriodDisplay(string? rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return "-";
        }

        var text = rawText.Trim();

        var timeMatch = TimeRangeRegex.Match(text);
        if (timeMatch.Success)
        {
            return NormalizeSpacing(timeMatch.Value.Replace('.', ':'));
        }

        var dayRangeMatch = DayRangeRegex.Match(text);
        if (dayRangeMatch.Success)
        {
            return NormalizeSpacing(dayRangeMatch.Value);
        }

        var daysPerWeekMatch = DaysPerWeekRegex.Match(text);
        if (daysPerWeekMatch.Success)
        {
            return NormalizeSpacing(daysPerWeekMatch.Value);
        }

        var hoursMatch = HoursRegex.Match(text);
        if (hoursMatch.Success)
        {
            return NormalizeSpacing(hoursMatch.Value)
                .Replace(" Hrs", " hours", StringComparison.OrdinalIgnoreCase)
                .Replace(" Hr", " hour", StringComparison.OrdinalIgnoreCase);
        }

        var rosterMatch = RosterRegex.Match(text);
        if (rosterMatch.Success)
        {
            return "Roster Based";
        }

        return "-";
    }

    public static string? GetAdditionalInformation(string? rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            return null;
        }

        var cleaned = rawText;

        cleaned = TimeRangeRegex.Replace(cleaned, " ");
        cleaned = DayRangeRegex.Replace(cleaned, " ");
        cleaned = DaysPerWeekRegex.Replace(cleaned, " ");
        cleaned = HoursRegex.Replace(cleaned, " ");
        cleaned = RosterRegex.Replace(cleaned, " ");

        cleaned = Regex.Replace(cleaned, "Position:\\s*", " ", RegexOptions.IgnoreCase);
        cleaned = Regex.Replace(cleaned, "Hours:\\s*", " ", RegexOptions.IgnoreCase);
        cleaned = Regex.Replace(cleaned, "^[-,:;\\s]+|[-,:;\\s]+$", string.Empty);
        cleaned = Regex.Replace(cleaned, "\\s{2,}", " ").Trim();
        cleaned = Regex.Replace(cleaned, "([,.;])\\s*([,.;])", "$1 ");
        cleaned = Regex.Replace(cleaned, "^([,.;-]\\s*)+", string.Empty);

        return string.IsNullOrWhiteSpace(cleaned) ? null : cleaned;
    }

    private static string NormalizeSpacing(string value)
    {
        return Regex.Replace(value.Trim(), "\\s+", " ");
    }
}
