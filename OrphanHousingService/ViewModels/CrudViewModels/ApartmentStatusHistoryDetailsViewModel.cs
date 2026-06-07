using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public class ApartmentStatusHistoryDetailsViewModel
    {
        public ApartmentStatusHistory Model { get; }

        public ApartmentStatusHistoryDetailsViewModel(ApartmentStatusHistory model)
        {
            Model = model;
        }
        public string Address => Model.Apartment.Address;
        public string Status => Model.DisplayStatus.ToString();
        public string? Basis => Model.Basis;
        public string? Comment => Model.Comment;
    }
}
