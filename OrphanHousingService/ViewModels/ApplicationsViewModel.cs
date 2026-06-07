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
    public partial class ApplicationsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly ApplicationService _applicationService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<Application> Applications { get; } = [];

        [ObservableProperty]
        private Application? selectedApplication;

        public ApplicationsViewModel(ApplicationService applicationService,
                                    IServiceProvider serviceProvider)
        {
            _applicationService = applicationService;
            _serviceProvider = serviceProvider;
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Applications.Clear();

            var items = await _applicationService.GetAllAsync();

            foreach (var item in items)
                Applications.Add(item);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApplicationView>();

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
            if (SelectedApplication == null)
                return;

            var window = _serviceProvider.GetRequiredService<ApplicationDetailsView>();
            DetailWindowHelper.Show(window, new ApplicationDetailsViewModel(SelectedApplication));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
