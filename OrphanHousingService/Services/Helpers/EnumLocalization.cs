using OrphanHousingService.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Helpers
{
    public static class EnumLocalization
    {
        public static string GetString(Enum value)
        {
            var typeName = value.GetType().Name;
            var key = $"{typeName}_{value}";

            var result = Enums.ResourceManager.GetString(
                key,
                System.Globalization.CultureInfo.CurrentUICulture);

            return result ?? value.ToString();
        }
    }
}
