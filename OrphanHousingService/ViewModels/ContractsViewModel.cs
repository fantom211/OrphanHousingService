using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels
{
    public partial class ContractsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ContractService _contractService;
        public ObservableCollection<Contract> Contracts { get; } = [];
        public ObservableCollection<UtilityDebt> UtilityDebts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        public ContractsViewModel(ContractService contractService, IServiceProvider serviceProvider)
        {
            _contractService = contractService;
            _ = LoadAsync();
            _serviceProvider = serviceProvider;
        }

        public async Task LoadAsync()
        {
            Contracts.Clear();

            var contracts = await _contractService.GetAllAsync();

            foreach (var a in contracts)
                Contracts.Add(a);
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
            {
                Contracts.Clear();
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
