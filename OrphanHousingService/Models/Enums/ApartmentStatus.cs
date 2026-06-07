using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Models.Enums
{
    public enum ApartmentStatus
    {
        Free,
        InSpecialFund,
        Distributed,
        SocialRent,
        Excluded
    }
}
