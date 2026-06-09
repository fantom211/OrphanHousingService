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
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels
{
    public partial class ContractsViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ContractService _contractService;
        private readonly MainViewModel _mainViewModel;
        private readonly ListCollectionManager<Contract> _listManager;

        public ICollectionView Contracts => _listManager.View;
        public ObservableCollection<UtilityDebt> UtilityDebts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private string? searchText;

        public ContractsViewModel(
            ContractService contractService,
            IServiceScopeFactory scopeFactory,
            MainViewModel mainViewModel)
        {
            _contractService = contractService;
            _scopeFactory = scopeFactory;
            _mainViewModel = mainViewModel;
            _listManager = new ListCollectionManager<Contract>(c => new[]
            {
                c.ContractNumber,
                c.Person?.FullName,
                c.Apartment?.Address
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Договоры");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var selectedId = SelectedContract?.Id;
            UtilityDebts.Clear();
            var contracts = await _contractService.GetAllAsync();
            _listManager.SetItems(contracts);
            SelectedContract = _listManager.RestoreSelection(selectedId, c => c.Id)
                ?? Contracts.Cast<Contract>().FirstOrDefault();
        }

        partial void OnSelectedContractChanged(Contract? value)
        {
            UtilityDebts.Clear();

            if (value?.UtilityDebts == null)
                return;

            foreach (var debt in value.UtilityDebts)
                UtilityDebts.Add(debt);
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddContractViewModel>();
            await vm.PrepareAsync();
            var window = new AddContractView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedContract == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddContractViewModel>();
            await vm.InitializeForEditAsync(SelectedContract);

            var window = new AddContractView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedContract == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedContract.ContractNumber))
                return;

            try
            {
                await _contractService.DeleteAsync(SelectedContract.Id);
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
            if (SelectedContract == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var window = new ContractDetailsView();
            var vm = scope.ServiceProvider.GetRequiredService<ContractDetailsViewModel>();
            vm.Initialize(SelectedContract);
            DetailWindowHelper.Show(window, vm);
        }

        [RelayCommand]
        private void NavigateToPerson()
        {
            if (SelectedContract == null)
                return;

            _mainViewModel.NavigateToPerson(SelectedContract.PersonId);
        }

        [RelayCommand]
        private void NavigateToApartment()
        {
            if (SelectedContract == null)
                return;

            _mainViewModel.NavigateToApartment(SelectedContract.ApartmentId);
        }

        [RelayCommand]
        private async void AddUtilityDebt()
        {
            if (SelectedContract == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddUtilityDebtViewModel>();
            await vm.InitializeForContractAsync(SelectedContract.Id);

            var window = new AddUtilityDebtView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void AddFamilyMember()
        {
            if (SelectedContract == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddFamilyMemberViewModel>();
            await vm.InitializeForContractAsync(SelectedContract.Id);

            var window = new AddFamilyMemberView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
