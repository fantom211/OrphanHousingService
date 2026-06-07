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
    public partial class ContractsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContractService _contractService;
        private readonly MainViewModel _mainViewModel;

        public ObservableCollection<Contract> Contracts { get; } = [];
        public ObservableCollection<UtilityDebt> UtilityDebts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        public ContractsViewModel(
            ContractService contractService,
            IServiceProvider serviceProvider,
            MainViewModel mainViewModel)
        {
            _contractService = contractService;
            _serviceProvider = serviceProvider;
            _mainViewModel = mainViewModel;
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            Contracts.Clear();

            var contracts = await _contractService.GetAllAsync();

            foreach (var contract in contracts)
                Contracts.Add(contract);
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
            var window = _serviceProvider.GetRequiredService<AddContractView>();
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
            if (SelectedContract == null)
                return;

            var window = _serviceProvider.GetRequiredService<ContractDetailsView>();
            DetailWindowHelper.Show(window, new ContractDetailsViewModel(SelectedContract));
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

            var vm = _serviceProvider.GetRequiredService<AddUtilityDebtViewModel>();
            vm.InitializeForContract(SelectedContract.Id);

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

            var vm = _serviceProvider.GetRequiredService<AddFamilyMemberViewModel>();
            vm.InitializeForContract(SelectedContract.Id);

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
