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
    public enum FamilyMemberCreationSource
    {
        Standalone,
        FromContract,
        FromPerson
    }

    public partial class AddFamilyMemberViewModel : ObservableObject
    {
        private readonly FamilyMemberService _familyMemberService;
        private readonly ContractService _contractService;
        private Guid? _prefilledContractId;
        private Guid? _editId;

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private string windowTitle = "Добавить члена семьи";

        [ObservableProperty]
        private FamilyMemberCreationSource creationSource = FamilyMemberCreationSource.Standalone;

        [ObservableProperty]
        private string? citizenDisplayName;

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private bool isContractLocked;

        public bool IsContractEditable => !IsContractLocked && !IsEditMode;
        public bool ShowCitizenInfo => CreationSource == FamilyMemberCreationSource.FromContract;
        public bool ShowPersonSourceInfo => CreationSource == FamilyMemberCreationSource.FromPerson;
        public bool ShowCitizenName => !string.IsNullOrWhiteSpace(CitizenDisplayName);
        public bool InitializationFailed { get; private set; }

        partial void OnCitizenDisplayNameChanged(string? value) =>
            OnPropertyChanged(nameof(ShowCitizenName));

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;

        [ObservableProperty]
        private RelationshipType relationshipType;

        public bool IsEditMode => _editId.HasValue;

        public IReadOnlyList<EnumItem<RelationshipType>> RelationshipTypes { get; } =
            EnumHelper.GetItems<RelationshipType>();

        public Action<bool>? CloseAction { get; set; }

        public AddFamilyMemberViewModel(
            FamilyMemberService familyMemberService,
            ContractService contractService)
        {
            _familyMemberService = familyMemberService;
            _contractService = contractService;
        }

        public async Task PrepareAsync()
        {
            if (CreationSource != FamilyMemberCreationSource.Standalone)
                return;

            await LoadAllContractsAsync();
        }

        public async Task InitializeForContractAsync(Guid contractId)
        {
            CreationSource = FamilyMemberCreationSource.FromContract;
            WindowTitle = "Добавить члена семьи (по договору)";
            _prefilledContractId = contractId;
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            OnPropertyChanged(nameof(ShowCitizenInfo));
            OnPropertyChanged(nameof(ShowPersonSourceInfo));
            await ApplyPrefilledContractAsync();
        }

        public async Task InitializeForPersonAsync(Guid personId)
        {
            CreationSource = FamilyMemberCreationSource.FromPerson;
            WindowTitle = "Добавить члена семьи (по гражданину)";
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            OnPropertyChanged(nameof(ShowCitizenInfo));
            OnPropertyChanged(nameof(ShowPersonSourceInfo));

            var contract = await _contractService.GetActiveContractForPersonAsync(personId);
            if (contract == null)
            {
                InitializationFailed = true;
                ValidationDialogHelper.ShowError(
                    new Exception("У гражданина нет активного договора"));
                return;
            }

            CitizenDisplayName = contract.Person?.FullName ?? "—";
            _prefilledContractId = contract.Id;
            await SetPrefilledContractAsync(contract);
        }

        public async Task InitializeForEditAsync(FamilyMember member)
        {
            _editId = member.Id;
            WindowTitle = "Редактировать члена семьи";
            FullName = member.FullName;
            BirthDate = member.BirthDate;
            RelationshipType = member.RelationshipType;
            _prefilledContractId = member.ContractId;
            IsContractLocked = true;
            OnPropertyChanged(nameof(IsContractEditable));
            await ApplyPrefilledContractAsync();
        }

        partial void OnIsContractLockedChanged(bool value)
        {
            OnPropertyChanged(nameof(IsContractEditable));
        }

        private async Task ApplyPrefilledContractAsync()
        {
            if (!_prefilledContractId.HasValue)
                return;

            var contract = await _contractService.GetByIdWithPersonAsync(_prefilledContractId.Value);
            if (contract == null)
                return;

            await SetPrefilledContractAsync(contract);
        }

        private Task SetPrefilledContractAsync(Contract contract)
        {
            Contracts.Clear();
            Contracts.Add(contract);
            SelectedContract = contract;

            if (CreationSource == FamilyMemberCreationSource.FromContract)
                CitizenDisplayName = contract.Person?.FullName ?? "—";

            return Task.CompletedTask;
        }

        private async Task LoadAllContractsAsync()
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
            try
            {
                var member = new FamilyMember
                {
                    Id = _editId ?? Guid.Empty,
                    ContractId = SelectedContract!.Id,
                    FullName = FullName,
                    BirthDate = BirthDate,
                    RelationshipType = RelationshipType
                };

                if (IsEditMode)
                    await _familyMemberService.UpdateAsync(member);
                else
                    await _familyMemberService.CreateAsync(member);

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
