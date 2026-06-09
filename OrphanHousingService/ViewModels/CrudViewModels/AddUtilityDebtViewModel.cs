using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddUtilityDebtViewModel : ObservableObject
    {
        private readonly UtilityDebtService _utilityDebtService;
        private readonly ContractService _contractService;
        private Guid? _prefilledContractId;
        private Guid? _editId;

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private string windowTitle = "Добавить долг";

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private bool isContractLocked;

        public bool IsContractEditable => !IsContractLocked && !IsEditMode;

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

        public bool IsPaidDateEnabled => Status == UtilityDebtStatus.Paid;
        public bool IsEditMode => _editId.HasValue;

        public IReadOnlyList<EnumItem<UtilityDebtStatus>> Statuses { get; } =
            EnumHelper.GetItems<UtilityDebtStatus>();

        public Action<bool>? CloseAction { get; set; }

        public AddUtilityDebtViewModel(
            UtilityDebtService utilityDebtService,
            ContractService contractService)
        {
            _utilityDebtService = utilityDebtService;
            _contractService = contractService;
        }

        public async Task PrepareAsync()
        {
            await LoadContractsAsync();
        }

        partial void OnStatusChanged(UtilityDebtStatus value)
        {
            if (value != UtilityDebtStatus.Paid)
                PaidDate = null;
            OnPropertyChanged(nameof(IsPaidDateEnabled));
        }

        partial void OnIsContractLockedChanged(bool value) =>
            OnPropertyChanged(nameof(IsContractEditable));

        public async Task InitializeForContractAsync(Guid contractId)
        {
            _prefilledContractId = contractId;
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            await LoadContractsAsync();
            ApplyPrefilledSelection();
        }

        public async Task InitializeForEditAsync(UtilityDebt debt)
        {
            _editId = debt.Id;
            WindowTitle = "Редактировать долг";
            Amount = debt.Amount;
            DebtDate = debt.DebtDate;
            PeriodStart = debt.PeriodStart;
            PeriodEnd = debt.PeriodEnd;
            Reason = debt.Reason;
            Status = debt.Status;
            PaidDate = debt.PaidDate;
            _prefilledContractId = debt.ContractId;
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            OnPropertyChanged(nameof(IsPaidDateEnabled));
            await LoadContractsAsync();
            ApplyPrefilledSelection();
        }

        private async Task LoadContractsAsync()
        {
            var contracts = await _contractService.GetAllAsync();
            Contracts.Clear();
            foreach (var contract in contracts)
                Contracts.Add(contract);
        }

        private void ApplyPrefilledSelection()
        {
            if (!_prefilledContractId.HasValue)
                return;

            SelectedContract = EntityComboHelper.FindById(Contracts, _prefilledContractId.Value);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (SelectedContract == null)
                {
                    ValidationDialogHelper.ShowError(new Exception("Выберите договор"));
                    return;
                }

                if (Amount <= 0)
                {
                    ValidationDialogHelper.ShowError(
                        new Exception("Сумма задолженности должна быть больше 0"));
                    return;
                }

                if (PeriodEnd.Date < PeriodStart.Date)
                {
                    ValidationDialogHelper.ShowError(
                        new Exception("Конец периода не может быть раньше начала"));
                    return;
                }

                var debt = new UtilityDebt
                {
                    Id = _editId ?? Guid.Empty,
                    ContractId = SelectedContract.Id,
                    Amount = Amount,
                    DebtDate = DebtDate.Date,
                    PeriodStart = PeriodStart.Date,
                    PeriodEnd = PeriodEnd.Date,
                    Reason = Reason,
                    Status = Status,
                    PaidDate = Status == UtilityDebtStatus.Paid ? PaidDate : null
                };

                if (IsEditMode)
                    await _utilityDebtService.UpdateAsync(debt);
                else
                    await _utilityDebtService.CreateAsync(debt);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void Cancel() => CloseAction?.Invoke(false);
    }
}
