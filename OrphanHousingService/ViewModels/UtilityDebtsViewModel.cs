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
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels
{
    public partial class UtilityDebtsViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly UtilityDebtService _utilityDebtService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ListCollectionManager<UtilityDebt> _listManager;

        public ICollectionView UtilityDebts => _listManager.View;

        [ObservableProperty]
        private UtilityDebt? selectedUtilityDebt;

        [ObservableProperty]
        private string? searchText;

        public UtilityDebtsViewModel(
            UtilityDebtService utilityDebtService,
            IServiceScopeFactory scopeFactory)
        {
            _utilityDebtService = utilityDebtService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<UtilityDebt>(d => new[]
            {
                d.Contract?.ContractNumber,
                d.Contract?.Person?.FullName,
                d.Contract?.Apartment?.Address,
                d.Reason
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Долги ЖКХ");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var items = await _utilityDebtService.GetAllAsync();
            _listManager.SetItems(items);
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddUtilityDebtViewModel>();
            var window = new AddUtilityDebtView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedUtilityDebt == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddUtilityDebtViewModel>();
            vm.InitializeForEdit(SelectedUtilityDebt);

            var window = new AddUtilityDebtView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedUtilityDebt == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete($"долг {SelectedUtilityDebt.Amount}"))
                return;

            try
            {
                await _utilityDebtService.DeleteAsync(SelectedUtilityDebt.Id);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedUtilityDebt == null)
                return;

            DetailWindowHelper.Show(
                new UtilityDebtDetailsView(),
                new UtilityDebtDetailsViewModel(SelectedUtilityDebt));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
