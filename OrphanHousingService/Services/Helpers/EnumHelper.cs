using OrphanHousingService.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Helpers
{
    public static class EnumHelper
    {
        public static List<EnumItem<T>> GetItems<T>() where T :struct,Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => new EnumItem<T>
                {
                    Value = x,
                    Display = EnumLocalization.GetString(x)
                })
                .ToList();
        }
    }
}
