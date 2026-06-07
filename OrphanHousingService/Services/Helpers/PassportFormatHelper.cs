using System.Text.RegularExpressions;

namespace OrphanHousingService.Services.Helpers
{
    public static class PassportFormatHelper
    {
        public const string Pattern = @"^\d{4}\s\d{6}$";
        public const string Placeholder = "2721 856782";
        public const string FormatDescription = "Формат: XXXX XXXXXX (4 цифры, пробел, 6 цифр)";

        public static bool IsValid(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return true;

            return Regex.IsMatch(value.Trim(), Pattern);
        }
    }
}
