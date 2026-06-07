using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
using System;
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
        private Person? selectedPerson;

        [ObservableProperty]
        private Apartment? selectedApartment;

        [ObservableProperty]
        private bool isPersonLocked;

        [ObservableProperty]
        private bool isApartmentLocked;

        public bool IsPersonEditable => !IsPersonLocked && !IsEditMode;
        public bool IsApartmentEditable => !IsApartmentLocked && !IsEditMode;

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

        public IReadOnlyList<EnumItem<ContractType>> ContractTypes { get; } =
            EnumHelper.GetItems<ContractType>();

        public IReadOnlyList<EnumItem<ContractStatus>> ContractStatuses { get; } =
            EnumHelper.GetItems<ContractStatus>();

        public AddContractViewModel(
            ContractService contractService,
            PersonService personService,
            ApartmentService apartmentService)
        {
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
        }

        partial void OnIsApartmentLockedChanged(bool value)
        {
            OnPropertyChanged(nameof(IsApartmentEditable));
        }

        partial void OnContractDateChanged(DateTime value)
        {
            if (!IsEditMode)
                EndDate = value.AddYears(5);
        }

        public void InitializeForPerson(Guid personId)
        {
            _prefilledPersonId = personId;
            IsPersonLocked = true;
            OnPropertyChanged(nameof(IsPersonEditable));
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
            ContractType = contract.ContractType;
            ContractNumber = contract.ContractNumber;
            ContractDate = contract.ContractDate;
            StartDate = contract.StartDate;
            EndDate = contract.EndDate;
            Status = contract.Status;
            OnPropertyChanged(nameof(IsPersonEditable));
            OnPropertyChanged(nameof(IsApartmentEditable));
            _ = ApplyPrefillsAsync(contract.PersonId, contract.ApartmentId);
        }

        private async Task ApplyPrefillsAsync(Guid? personId = null, Guid? apartmentId = null)
        {
            if (People.Count == 0)
                await LoadAsync();

            var targetPersonId = personId ?? _prefilledPersonId;
            var targetApartmentId = apartmentId ?? _prefilledApartmentId;

            if (targetPersonId.HasValue)
                SelectedPerson = People.FirstOrDefault(p => p.Id == targetPersonId.Value);

            if (targetApartmentId.HasValue)
                SelectedApartment = Apartments.FirstOrDefault(a => a.Id == targetApartmentId.Value);
        }

        private async Task LoadAsync()
        {
            var people = await _personService.GetAllAsync();
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
                        ContractDate = ContractDate,
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Status = Status
                    };

                    await _contractService.UpdateAsync(contract);
                }
                else
                {
                    var contract = new Contract
                    {
                        PersonId = SelectedPerson!.Id,
                        ApartmentId = SelectedApartment!.Id,
                        ContractType = ContractType,
                        ContractNumber = string.IsNullOrWhiteSpace(ContractNumber)
                            ? SuggestedContractNumber
                            : ContractNumber,
                        ContractDate = ContractDate,
                        StartDate = StartDate,
                        EndDate = EndDate,
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
