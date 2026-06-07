using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.Details
{
    public partial class ContractHistoryViewModel
    {
        private readonly ContractService _contractService;

        public ObservableCollection<Contract> Contracts { get; } = new();

        public ContractHistoryViewModel(ContractService contractService, Guid personId)
        {
            _contractService = contractService;

            _ = Load(personId);
        }

        [RelayCommand]
        private async Task Load(Guid contractId)
        {
            var history =  await _contractService.GetByIdAsync(contractId);
        }
    }
}
