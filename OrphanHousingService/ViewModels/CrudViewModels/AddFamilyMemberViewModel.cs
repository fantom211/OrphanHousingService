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
    public partial class AddFamilyMemberViewModel : ObservableObject
    {
        private readonly FamilyMemberService _familyMemberService;
        private readonly ContractService _contractService;
        private Guid? _prefilledContractId;

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private bool isContractLocked;

        public bool IsContractEditable => !IsContractLocked;

        [ObservableProperty]
        private string fullName = string.Empty;

        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;

        [ObservableProperty]
        private RelationshipType relationshipType;

        public IReadOnlyList<EnumItem<RelationshipType>> RelationshipTypes { get; } =
            EnumHelper.GetItems<RelationshipType>();

        public Action<bool>? CloseAction { get; set; }

        public AddFamilyMemberViewModel(
            FamilyMemberService familyMemberService,
            ContractService contractService)
        {
            _familyMemberService = familyMemberService;
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
            var member = new FamilyMember
            {
                ContractId = SelectedContract!.Id,
                FullName = FullName,
                BirthDate = BirthDate,
                RelationshipType = RelationshipType
            };

            await _familyMemberService.CreateAsync(member);
            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
