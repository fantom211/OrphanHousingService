using OrphanHousingService.Services.Helpers;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OrphanHousingService.Models.Helpers
{
    public class ComboBoxSelectionDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return string.Empty;

            if (value is Enum enumValue)
                return EnumLocalization.GetString(enumValue);

            foreach (var propertyName in new[]
                     {
                         "Display", "FullName", "Address",
                         "ApplicationNumber", "ContractNumber"
                     })
            {
                var property = value.GetType().GetProperty(propertyName);
                if (property?.GetValue(value) is string text && !string.IsNullOrWhiteSpace(text))
                    return text;
            }

            return value.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
