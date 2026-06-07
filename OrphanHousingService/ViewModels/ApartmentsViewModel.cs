using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels
{
    public partial class ApartmentsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly ApartmentService _apartmentService;
        private readonly IServiceProvider _serviceProvider;
        public ObservableCollection<Apartment> Apartments { get; } = [];

        [ObservableProperty]
        private Apartment? selectedApartment;

        public ApartmentsViewModel(ApartmentService apartmentService, IServiceProvider serviceProvider)
        {
            _apartmentService = apartmentService;
            _ = LoadAsync();
            _serviceProvider = serviceProvider;
        }

        public async Task LoadAsync()
        {
            Apartments.Clear();

            var apartments = await _apartmentService.GetAllAsync();

            foreach (var apartment in apartments)
                Apartments.Add(apartment);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApartmentView>();

            if (window.ShowDialog() == true)
            {
                Apartments.Clear();
                await LoadAsync();
            }
        }

        [RelayCommand]
        private void Edit()
        {

        }

        [RelayCommand]
        private void Delete()
        {

        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
