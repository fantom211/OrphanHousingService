using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddContractViewModel : ObservableObject
    {
        private readonly ContractService _contractService;
        private readonly PersonService _personService;
        private readonly ApartmentService _apartmentService;

        public ObservableCollection<Person> People { get; } = [];
        public ObservableCollection<Apartment> Apartments { get; } = [];

        [ObservableProperty]
        private Person? selectedPerson;

        [ObservableProperty]
        private Apartment? selectedApartment;

        [ObservableProperty]
        private ContractType contractType;

        [ObservableProperty]
        private string contractNumber = string.Empty;

        [ObservableProperty]
        private DateTime contractDate = DateTime.Today;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private ContractStatus status;

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

            _ = LoadAsync();
        }

        partial void OnContractDateChanged(DateTime value)
        {
            EndDate = value.AddYears(5);
        }

        private async Task LoadAsync()
        {
            var people = await _personService.GetAllAsync();
            var apartments = await _apartmentService.GetAllAsync();

            foreach (var p in people)
                People.Add(p);

            foreach (var a in apartments)
                Apartments.Add(a);
        }

        [RelayCommand]
        private async Task Save()
        {
            var contract = new Contract
            {
                PersonId = SelectedPerson!.Id,
                ApartmentId = SelectedApartment!.Id,
                ContractType = ContractType,
                ContractNumber = ContractNumber,
                ContractDate = ContractDate,
                StartDate = StartDate,
                EndDate = EndDate,
                Status = Status
            };

            await _contractService.CreateAsync(contract);
            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
