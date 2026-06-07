using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Helpers
{
    public static class PhoneNormalizer
    {
        public static string NormalizeRussianPhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            // убираем всё кроме цифр
            var digits = Regex.Replace(input, @"\D", "");

            // 8XXXXXXXXXX → 7XXXXXXXXXX
            if (digits.Length == 11 && digits.StartsWith("8"))
                digits = "7" + digits[1..];

            // если уже начинается с 7 — ок
            if (digits.Length == 11 && digits.StartsWith("7"))
                return "+7" + digits[1..];

            return input; // вернём как есть для валидации (ошибка)
        }

        public static bool IsValidRussianPhone(string input)
        {
            var digits = Regex.Replace(input ?? "", @"\D", "");

            if (digits.Length == 11 && (digits.StartsWith("7") || digits.StartsWith("8")))
                return true;

            return false;
        }
    }
}
