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
            {
                Applications.Clear();
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
