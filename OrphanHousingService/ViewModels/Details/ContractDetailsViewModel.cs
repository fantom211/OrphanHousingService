using CommunityToolkit.Mvvm.ComponentModel;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels.Details
{
    public partial class ContractDetailsViewModel : ObservableObject
    {
        private readonly ContractHistoryService _historyService;

        [ObservableProperty]
        private Contract contract = null!;

        public ObservableCollection<ContractHistory> History { get; } = [];

        public ContractDetailsViewModel(ContractHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async void Initialize(Contract contract)
        {
            Contract = contract;
            History.Clear();

            var items = await _historyService.GetByContractIdAsync(contract.Id);
            foreach (var item in items)
                History.Add(item);
        }
    }
}
