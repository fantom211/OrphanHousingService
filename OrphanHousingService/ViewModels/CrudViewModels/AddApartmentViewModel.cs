using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Views.CrudViews
{
    public partial class AddApartmentViewModel : ObservableObject
    {
        private readonly ApartmentService _apartmentService;
        public IReadOnlyList<EnumItem<ApartmentStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApartmentStatus>();

        [ObservableProperty]
        private string address = string.Empty;

        [ObservableProperty]
        private string? cadastralNumber;

        [ObservableProperty]
        private decimal area;

        [ObservableProperty]
        private int roomsCount;

        [ObservableProperty]
        private ApartmentStatus currentStatus;

        [ObservableProperty]
        private DateTime? includedToFundDate;

        [ObservableProperty]
        private string? inclussionOrderNumber;

        [ObservableProperty]
        private DateTime? inclussionOrderDate;

        public Action<bool>? CloseAction { get; set; }

        public AddApartmentViewModel(ApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        [RelayCommand]
        private async Task Save()
        {
            var apartment = new Apartment
            {
                Address = Address,
                CadastralNumber = CadastralNumber,
                Area = Area,
                RoomsCount = RoomsCount,
                CurrentStatus = CurrentStatus,
                IncludedToFundDate = IncludedToFundDate,
                InclussionOrderNumber = InclussionOrderNumber,
                InclussionOrderDate = InclussionOrderDate
            };

            await _apartmentService.CreateAsync(apartment);

            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
