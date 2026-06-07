using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddApartmentStatusHistoryViewModel : ObservableObject
    {
        private readonly ApartmentStatusHistoryService _historyService;
        private readonly ApartmentService _apartmentService;

        public ObservableCollection<Apartment> Apartments { get; } = [];

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

        public Action<bool>? CloseAction { get; set; }

        public IReadOnlyList<EnumItem<ApartmentStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApartmentStatus>();

        public AddApartmentStatusHistoryViewModel(
            ApartmentStatusHistoryService historyService,
            ApartmentService apartmentService)
        {
            _historyService = historyService;
            _apartmentService = apartmentService;

            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var apartments = await _apartmentService.GetAllAsync();

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
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Height = double.NaN; // Auto
                tb.Measure(new Size(tb.ActualWidth, double.PositiveInfinity));

                tb.Height = Math.Min(tb.DesiredSize.Height + 10, 120);
            }
        }
    }
}
