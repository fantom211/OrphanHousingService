using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.CrudViewModels;
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
    public partial class ApartmentStatusHistoriesViewModel : ObservableObject, ICrudViewModel
    {
        private readonly ApartmentStatusHistoryService _historyService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<ApartmentStatusHistory> Histories { get; } = [];

        [ObservableProperty]
        private ApartmentStatusHistory? selectedHistory;

        public ApartmentStatusHistoriesViewModel(
            ApartmentStatusHistoryService historyService,
            IServiceProvider serviceProvider)
        {
            _historyService = historyService;
            _serviceProvider = serviceProvider;

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Histories.Clear();

            var histories = await _historyService.GetAllAsync();

            foreach (var history in histories)
                Histories.Add(history);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApartmentStatusHistoryView>();

            if (window.ShowDialog() == true)
            {
                Histories.Clear();
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

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedHistory == null)
                return;

            var window = _serviceProvider.GetRequiredService<ApartmentStatusHistoryDetailsView>();
            window.DataContext = new ApartmentStatusHistoryDetailsViewModel(SelectedHistory);

            window.Owner = System.Windows.Application.Current.MainWindow;
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;

            window.ShowDialog();

        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
