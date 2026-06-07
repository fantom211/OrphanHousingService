using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
using System;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddApartmentViewModel : ObservableObject
    {
        private readonly ApartmentService _apartmentService;
        private Guid? _editId;

        public IReadOnlyList<EnumItem<ApartmentStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApartmentStatus>();

        [ObservableProperty]
        private string windowTitle = "Добавить квартиру";

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

        public bool IsEditMode => _editId.HasValue;
        public bool IsStatusEditable => !IsEditMode;

        public Action<bool>? CloseAction { get; set; }

        public AddApartmentViewModel(ApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        public void InitializeForEdit(Apartment apartment)
        {
            _editId = apartment.Id;
            WindowTitle = "Редактировать квартиру";
            Address = apartment.Address;
            CadastralNumber = apartment.CadastralNumber;
            Area = apartment.Area;
            RoomsCount = apartment.RoomsCount;
            CurrentStatus = apartment.CurrentStatus;
            IncludedToFundDate = apartment.IncludedToFundDate;
            InclussionOrderNumber = apartment.InclussionOrderNumber;
            InclussionOrderDate = apartment.InclussionOrderDate;
            OnPropertyChanged(nameof(IsStatusEditable));
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                var apartment = new Apartment
                {
                    Id = _editId ?? Guid.Empty,
                    Address = Address,
                    CadastralNumber = CadastralNumber,
                    Area = Area,
                    RoomsCount = RoomsCount,
                    CurrentStatus = CurrentStatus,
                    IncludedToFundDate = IncludedToFundDate,
                    InclussionOrderNumber = InclussionOrderNumber,
                    InclussionOrderDate = InclussionOrderDate
                };

                if (IsEditMode)
                    await _apartmentService.UpdateAsync(apartment);
                else
                    await _apartmentService.CreateAsync(apartment);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
