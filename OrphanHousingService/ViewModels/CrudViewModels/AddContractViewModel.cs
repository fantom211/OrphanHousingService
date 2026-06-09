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
    public partial class AddContractViewModel : ObservableObject
    {
        private readonly ContractService _contractService;
        private readonly PersonService _personService;
        private readonly ApartmentService _apartmentService;
        private Guid? _editId;
        private Guid? _prefilledPersonId;
        private Guid? _prefilledApartmentId;

        public ObservableCollection<Person> People { get; } = [];
        public ObservableCollection<Apartment> Apartments { get; } = [];

        [ObservableProperty]
        private string windowTitle = "Добавить договор";

        [ObservableProperty]
        private ContractCounterpartyType counterpartyType = ContractCounterpartyType.Citizen;

        [ObservableProperty]
        private Person? selectedPerson;

        [ObservableProperty]
        private Apartment? selectedApartment;

        [ObservableProperty]
        private bool isPersonLocked;

        [ObservableProperty]
        private bool isApartmentLocked;

        public bool IsPersonEditable => !IsPersonLocked && !IsEditMode;
        public bool IsApartmentEditable => !IsApartmentLocked && !IsEditMode;
        public bool IsCounterpartyTypeEditable => !IsEditMode && !IsPersonLocked;
        public bool IsPersonSelectorVisible => CounterpartyType == ContractCounterpartyType.Citizen;
        public bool IsSystemCounterpartyVisible => !IsPersonSelectorVisible;

        public string CounterpartyDisplayName => CounterpartyType switch
        {
            ContractCounterpartyType.SpecialHousingFund =>
                EnumLocalization.GetString(ContractCounterpartyType.SpecialHousingFund),
            ContractCounterpartyType.SocialRent =>
                EnumLocalization.GetString(ContractCounterpartyType.SocialRent),
            _ => SelectedPerson?.FullName ?? string.Empty
        };

        [ObservableProperty]
        private ContractType contractType;

        [ObservableProperty]
        private string contractNumber = string.Empty;

        [ObservableProperty]
        private string suggestedContractNumber = string.Empty;

        [ObservableProperty]
        private DateTime contractDate = DateTime.Today;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private ContractStatus status;

        public bool IsEditMode => _editId.HasValue;
        public Action<bool>? CloseAction { get; set; }

        public IReadOnlyList<EnumItem<ContractCounterpartyType>> CounterpartyTypes { get; } =
            EnumHelper.GetItems<ContractCounterpartyType>();

        public IReadOnlyList<EnumItem<ContractType>> ContractTypes { get; } =
            EnumHelper.GetItems<ContractType>();

        public IReadOnlyList<EnumItem<ContractStatus>> ContractStatuses { get; } =
            EnumHelper.GetItems<ContractStatus>();

        public AddContractViewModel(
            ContractService contractService,
            PersonService personService,
            ApartmentService apartmentService)
        {
            contractType = ContractType.SpecializedRent;
            status = ContractStatus.Active;
            endDate = startDate.AddYears(5);
            _contractService = contractService;
            _personService = personService;
            _apartmentService = apartmentService;
            SuggestedContractNumber = _contractService.GenerateNumber();
        }

        public async Task PrepareAsync()
        {
            await LoadLookupsAsync();
        }

        public async Task InitializeForPersonAsync(Guid personId)
        {
            _prefilledPersonId = personId;
            CounterpartyType = ContractCounterpartyType.Citizen;
            IsPersonLocked = true;
            NotifyEditableFlags();
            await LoadLookupsAsync();
            ApplyPrefilledSelection();
        }

        public async Task InitializeForApartmentAsync(Guid apartmentId)
        {
            _prefilledApartmentId = apartmentId;
            IsApartmentLocked = true;
            OnPropertyChanged(nameof(IsApartmentEditable));
            await LoadLookupsAsync();
            ApplyPrefilledSelection();
        }

        public async Task InitializeForEditAsync(Contract contract)
        {
            _editId = contract.Id;
            WindowTitle = "Редактировать договор";
            CounterpartyType = ResolveCounterpartyType(contract.PersonId);
            ContractType = contract.ContractType;
            ContractNumber = contract.ContractNumber;
            ContractDate = contract.ContractDate;
            StartDate = contract.StartDate;
            EndDate = contract.EndDate;
            Status = contract.Status;
            _prefilledPersonId = contract.PersonId;
            _prefilledApartmentId = contract.ApartmentId;
            NotifyEditableFlags();
            await LoadLookupsAsync();
            ApplyPrefilledSelection();
        }

        partial void OnIsPersonLockedChanged(bool value) => NotifyEditableFlags();
        partial void OnIsApartmentLockedChanged(bool value) => OnPropertyChanged(nameof(IsApartmentEditable));

        partial void OnCounterpartyTypeChanged(ContractCounterpartyType value)
        {
            ApplyCounterpartyDefaults(value);
            OnPropertyChanged(nameof(IsPersonSelectorVisible));
            OnPropertyChanged(nameof(IsSystemCounterpartyVisible));
            OnPropertyChanged(nameof(CounterpartyDisplayName));
        }

        partial void OnSelectedPersonChanged(Person? value) =>
            OnPropertyChanged(nameof(CounterpartyDisplayName));

        partial void OnContractDateChanged(DateTime value)
        {
            if (!IsEditMode)
                EndDate = value.AddYears(5);
        }

        private void NotifyEditableFlags()
        {
            OnPropertyChanged(nameof(IsPersonEditable));
            OnPropertyChanged(nameof(IsApartmentEditable));
            OnPropertyChanged(nameof(IsCounterpartyTypeEditable));
            OnPropertyChanged(nameof(IsPersonSelectorVisible));
            OnPropertyChanged(nameof(IsSystemCounterpartyVisible));
        }

        private static ContractCounterpartyType ResolveCounterpartyType(Guid personId)
        {
            if (personId == SystemEntityIds.SpecialHousingFund)
                return ContractCounterpartyType.SpecialHousingFund;
            if (personId == SystemEntityIds.SocialRent)
                return ContractCounterpartyType.SocialRent;
            return ContractCounterpartyType.Citizen;
        }

        private void ApplyCounterpartyDefaults(ContractCounterpartyType value)
        {
            ContractType = value switch
            {
                ContractCounterpartyType.SpecialHousingFund => ContractType.SpecializedRent,
                ContractCounterpartyType.SocialRent => ContractType.SocialRent,
                _ => ContractType
            };
        }

        private Guid ResolvePersonId() => CounterpartyType switch
        {
            ContractCounterpartyType.SpecialHousingFund => SystemEntityIds.SpecialHousingFund,
            ContractCounterpartyType.SocialRent => SystemEntityIds.SocialRent,
            _ => SelectedPerson!.Id
        };

        private async Task LoadLookupsAsync()
        {
            var people = await _personService.GetCitizensAsync();
            var apartments = await _apartmentService.GetAllAsync();

            People.Clear();
            Apartments.Clear();

            foreach (var person in people)
                People.Add(person);
            foreach (var apartment in apartments)
                Apartments.Add(apartment);
        }

        private void ApplyPrefilledSelection()
        {
            if (_prefilledPersonId.HasValue && CounterpartyType == ContractCounterpartyType.Citizen)
                SelectedPerson = EntityComboHelper.FindById(People, _prefilledPersonId.Value);

            if (_prefilledApartmentId.HasValue)
                SelectedApartment = EntityComboHelper.FindById(Apartments, _prefilledApartmentId.Value);

            OnPropertyChanged(nameof(CounterpartyDisplayName));
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (IsEditMode)
                {
                    await _contractService.UpdateAsync(new Contract
                    {
                        Id = _editId!.Value,
                        ContractType = ContractType,
                        ContractNumber = string.IsNullOrWhiteSpace(ContractNumber)
                            ? SuggestedContractNumber
                            : ContractNumber,
                        ContractDate = ContractDate.Date,
                        StartDate = StartDate.Date,
                        EndDate = EndDate.Date,
                        Status = Status
                    });
                }
                else
                {
                    await _contractService.CreateAsync(new Contract
                    {
                        PersonId = ResolvePersonId(),
                        ApartmentId = SelectedApartment!.Id,
                        ContractType = ContractType,
                        ContractNumber = string.IsNullOrWhiteSpace(ContractNumber)
                            ? SuggestedContractNumber
                            : ContractNumber,
                        ContractDate = ContractDate.Date,
                        StartDate = StartDate.Date,
                        EndDate = EndDate.Date,
                        Status = Status
                    });
                }

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
