using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddApartmentStatusHistoryViewModel : ObservableObject
    {
        private readonly ApartmentStatusHistoryService _historyService;
        private readonly ApartmentService _apartmentService;

        public ObservableCollection<Apartment> Apartments { get; } = [];

        [ObservableProperty]
        private string windowTitle = "Добавить запись истории статуса";

        [ObservableProperty]
        private Apartment? selectedApartment;

        [ObservableProperty]
        private DateTime changeDate = DateTime.Today;

        [ObservableProperty]
        private ApartmentStatus status;

        [ObservableProperty]
        private string? basis;

        [ObservableProperty]
        private string? comment;

        public IReadOnlyList<EnumItem<ApartmentStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApartmentStatus>();

        public Action<bool>? CloseAction { get; set; }

        public AddApartmentStatusHistoryViewModel(
            ApartmentStatusHistoryService historyService,
            ApartmentService apartmentService)
        {
            _historyService = historyService;
            _apartmentService = apartmentService;
        }

        public async Task PrepareAsync()
        {
            var apartments = await _apartmentService.GetAllAsync();
            Apartments.Clear();
            foreach (var apartment in apartments)
                Apartments.Add(apartment);
        }

        [RelayCommand]
        private async Task Save()
        {
            var history = new ApartmentStatusHistory
            {
                ApartmentId = SelectedApartment!.Id,
                ChangeDate = ChangeDate,
                Status = Status,
                Basis = Basis,
                Comment = Comment
            };

            await _historyService.CreateAsync(history);
            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel() => CloseAction?.Invoke(false);
    }
}
