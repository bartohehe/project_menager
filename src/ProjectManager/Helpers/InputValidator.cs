using System.Text.RegularExpressions;

namespace ProjectManager.Helpers;

public static partial class InputValidator
{
    private static readonly char[] DangerousChars = ['$', '{', '}'];

    public static (bool IsValid, string? Error) ValidateProjectName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "Nazwa projektu jest wymagana.");

        if (name.Length > 200)
            return (false, "Nazwa projektu nie może przekraczać 200 znaków.");

        if (ContainsDangerousChars(name))
            return (false, "Nazwa zawiera niedozwolone znaki.");

        return (true, null);
    }

    public static (bool IsValid, string? Error) ValidateDescription(string? description)
    {
        if (description is not null && description.Length > 10000)
            return (false, "Opis nie może przekraczać 10000 znaków.");

        return (true, null);
    }

    public static (bool IsValid, string? Error) ValidatePrice(string? priceText)
    {
        if (string.IsNullOrWhiteSpace(priceText))
            return (true, null);

        if (!decimal.TryParse(priceText, out var price))
            return (false, "Nieprawidłowy format ceny.");

        if (price < 0)
            return (false, "Cena nie może być ujemna.");

        if (price > 999_999_999)
            return (false, "Cena jest zbyt wysoka.");

        return (true, null);
    }

    public static string SanitizeString(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        var sanitized = input.Trim();
        sanitized = ControlCharsRegex().Replace(sanitized, string.Empty);
        return sanitized;
    }

    private static bool ContainsDangerousChars(string input)
    {
        return input.IndexOfAny(DangerousChars) >= 0;
    }

    [GeneratedRegex(@"[\x00-\x08\x0B\x0C\x0E-\x1F]")]
    private static partial Regex ControlCharsRegex();
}
