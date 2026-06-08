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

            _ = LoadAsync();

        }



        partial void OnIsPersonLockedChanged(bool value)

        {

            OnPropertyChanged(nameof(IsPersonEditable));

            OnPropertyChanged(nameof(IsCounterpartyTypeEditable));

        }



        partial void OnIsApartmentLockedChanged(bool value)

        {

            OnPropertyChanged(nameof(IsApartmentEditable));

        }



        partial void OnCounterpartyTypeChanged(ContractCounterpartyType value)

        {

            ApplyCounterpartyDefaults(value);

            OnPropertyChanged(nameof(IsPersonSelectorVisible));

            OnPropertyChanged(nameof(IsSystemCounterpartyVisible));

            OnPropertyChanged(nameof(CounterpartyDisplayName));

        }



        partial void OnSelectedPersonChanged(Person? value)

        {

            OnPropertyChanged(nameof(CounterpartyDisplayName));

        }



        partial void OnContractDateChanged(DateTime value)

        {

            if (!IsEditMode)

                EndDate = value.AddYears(5);

        }



        public void InitializeForPerson(Guid personId)

        {

            _prefilledPersonId = personId;

            CounterpartyType = ContractCounterpartyType.Citizen;

            IsPersonLocked = true;

            OnPropertyChanged(nameof(IsPersonEditable));

            OnPropertyChanged(nameof(IsCounterpartyTypeEditable));

            OnPropertyChanged(nameof(IsPersonSelectorVisible));

            OnPropertyChanged(nameof(IsSystemCounterpartyVisible));

            _ = ApplyPrefillsAsync();

        }



        public void InitializeForApartment(Guid apartmentId)

        {

            _prefilledApartmentId = apartmentId;

            IsApartmentLocked = true;

            OnPropertyChanged(nameof(IsApartmentEditable));

            _ = ApplyPrefillsAsync();

        }



        public void InitializeForEdit(Contract contract)

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

            OnPropertyChanged(nameof(IsPersonEditable));

            OnPropertyChanged(nameof(IsApartmentEditable));

            OnPropertyChanged(nameof(IsCounterpartyTypeEditable));

            OnPropertyChanged(nameof(IsPersonSelectorVisible));

            OnPropertyChanged(nameof(IsSystemCounterpartyVisible));

            _ = ApplyPrefillsAsync(contract.PersonId, contract.ApartmentId);

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

            switch (value)

            {

                case ContractCounterpartyType.SpecialHousingFund:

                    ContractType = ContractType.SpecializedRent;

                    break;

                case ContractCounterpartyType.SocialRent:

                    ContractType = ContractType.SocialRent;

                    break;

            }

        }



        private Guid ResolvePersonId()

        {

            return CounterpartyType switch

            {

                ContractCounterpartyType.SpecialHousingFund => SystemEntityIds.SpecialHousingFund,

                ContractCounterpartyType.SocialRent => SystemEntityIds.SocialRent,

                _ => SelectedPerson!.Id

            };

        }



        private async Task ApplyPrefillsAsync(Guid? personId = null, Guid? apartmentId = null)

        {

            if (People.Count == 0)

                await LoadAsync();



            var targetPersonId = personId ?? _prefilledPersonId;

            var targetApartmentId = apartmentId ?? _prefilledApartmentId;



            if (targetPersonId.HasValue && CounterpartyType == ContractCounterpartyType.Citizen)

                SelectedPerson = People.FirstOrDefault(p => p.Id == targetPersonId.Value);



            if (targetApartmentId.HasValue)

                SelectedApartment = Apartments.FirstOrDefault(a => a.Id == targetApartmentId.Value);



            OnPropertyChanged(nameof(CounterpartyDisplayName));

        }



        private async Task LoadAsync()

        {

            var people = await _personService.GetCitizensAsync();

            var apartments = await _apartmentService.GetAllAsync();



            People.Clear();

            Apartments.Clear();



            foreach (var p in people)

                People.Add(p);



            foreach (var a in apartments)

                Apartments.Add(a);



            await ApplyPrefillsAsync();

        }



        [RelayCommand]

        private async Task Save()

        {

            try

            {

                if (IsEditMode)

                {

                    var contract = new Contract

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

                    };



                    await _contractService.UpdateAsync(contract);

                }

                else

                {

                    var contract = new Contract

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

                    };



                    await _contractService.CreateAsync(contract);

                }



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


