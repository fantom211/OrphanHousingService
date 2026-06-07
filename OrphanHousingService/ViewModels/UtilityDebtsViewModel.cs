using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.CrudViewModels;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels
{
    public partial class UtilityDebtsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly UtilityDebtService _utilityDebtService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<UtilityDebt> UtilityDebts { get; } = [];

        [ObservableProperty]
        private UtilityDebt? selectedUtilityDebt;

        public UtilityDebtsViewModel(
            UtilityDebtService utilityDebtService,
            IServiceProvider serviceProvider)
        {
            _utilityDebtService = utilityDebtService;
            _serviceProvider = serviceProvider;
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            UtilityDebts.Clear();

            var items = await _utilityDebtService.GetAllAsync();

            foreach (var item in items)
                UtilityDebts.Add(item);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddUtilityDebtView>();

            window.Owner = System.Windows.Application.Current.MainWindow;

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
            if (SelectedUtilityDebt == null)
                return;

            var window = _serviceProvider.GetRequiredService<UtilityDebtDetailsView>();
            DetailWindowHelper.Show(window, new UtilityDebtDetailsViewModel(SelectedUtilityDebt));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
