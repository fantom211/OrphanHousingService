using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Models.Helpers
{
    public class EnumItem<T> where T : struct, Enum
    {
        public T Value { get; set; }
        public string Display { get; set; } = null!;
    }
}
