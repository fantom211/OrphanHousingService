using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.Details
{
    public class ApartmentDetailsViewModel
    {
        public Apartment Apartment { get; }

        public ApartmentDetailsViewModel(Apartment apartment)
        {
            Apartment = apartment;
        }
    }
}
