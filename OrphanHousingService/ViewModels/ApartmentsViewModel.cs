using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels
{
    public partial class ApartmentsViewModel : ObservableObject, ICrudViewModel, ISelectableViewModel
    {
        private readonly ApartmentService _apartmentService;
        private readonly IServiceProvider _serviceProvider;
        private Guid? _pendingSelectionId;

        public ObservableCollection<Apartment> Apartments { get; } = [];

        [ObservableProperty]
        private Apartment? selectedApartment;

        public ApartmentsViewModel(ApartmentService apartmentService, IServiceProvider serviceProvider)
        {
            _apartmentService = apartmentService;
            _serviceProvider = serviceProvider;
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Apartments.Clear();

            var apartments = await _apartmentService.GetAllAsync();

            foreach (var apartment in apartments)
                Apartments.Add(apartment);

            if (_pendingSelectionId.HasValue)
            {
                SelectById(_pendingSelectionId.Value);
                _pendingSelectionId = null;
            }
        }

        public void SelectById(Guid id)
        {
            SelectedApartment = Apartments.FirstOrDefault(a => a.Id == id);

            if (SelectedApartment == null)
                _pendingSelectionId = id;
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApartmentView>();

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private void Edit()
        {
        }

        [RelayCommand]
        private void Delete()
        {
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedApartment == null)
                return;

            var window = _serviceProvider.GetRequiredService<ApartmentDetailsView>();
            DetailWindowHelper.Show(window, new ApartmentDetailsViewModel(SelectedApartment));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
