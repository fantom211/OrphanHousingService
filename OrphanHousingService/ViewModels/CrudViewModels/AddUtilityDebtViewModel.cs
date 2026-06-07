using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddUtilityDebtViewModel : ObservableObject
    {
        private readonly UtilityDebtService _utilityDebtService;
        private readonly ContractService _contractService;
        private Guid? _prefilledContractId;

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private bool isContractLocked;

        public bool IsContractEditable => !IsContractLocked;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime debtDate = DateTime.Today;

        [ObservableProperty]
        private DateTime periodStart = DateTime.Today;

        [ObservableProperty]
        private DateTime periodEnd = DateTime.Today;

        [ObservableProperty]
        private string? reason;

        [ObservableProperty]
        private UtilityDebtStatus status = UtilityDebtStatus.Unpaid;

        [ObservableProperty]
        private DateTime? paidDate;

        public IReadOnlyList<EnumItem<UtilityDebtStatus>> Statuses { get; } =
            EnumHelper.GetItems<UtilityDebtStatus>();

        public Action<bool>? CloseAction { get; set; }

        public AddUtilityDebtViewModel(
            UtilityDebtService utilityDebtService,
            ContractService contractService)
        {
            _utilityDebtService = utilityDebtService;
            _contractService = contractService;
            _ = LoadAsync();
        }

        public void InitializeForContract(Guid contractId)
        {
            _prefilledContractId = contractId;
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            _ = ApplyPrefilledContractAsync();
        }

        partial void OnIsContractLockedChanged(bool value)
        {
            OnPropertyChanged(nameof(IsContractEditable));
        }

        private async Task ApplyPrefilledContractAsync()
        {
            if (!_prefilledContractId.HasValue)
                return;

            if (Contracts.Count == 0)
                await LoadAsync();

            SelectedContract = Contracts.FirstOrDefault(c => c.Id == _prefilledContractId.Value);
        }

        private async Task LoadAsync()
        {
            var contracts = await _contractService.GetAllAsync();

            Contracts.Clear();
            foreach (var contract in contracts)
                Contracts.Add(contract);

            if (_prefilledContractId.HasValue)
                SelectedContract = Contracts.FirstOrDefault(c => c.Id == _prefilledContractId.Value);
        }

        [RelayCommand]
        private async Task Save()
        {
            var debt = new UtilityDebt
            {
                ContractId = SelectedContract!.Id,
                Amount = Amount,
                DebtDate = DebtDate,
                PeriodStart = PeriodStart,
                PeriodEnd = PeriodEnd,
                Reason = Reason,
                Status = Status,
                PaidDate = Status == UtilityDebtStatus.Paid ? PaidDate : null
            };

            await _utilityDebtService.CreateAsync(debt);
            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
