using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddPersonViewModel : ObservableObject
    {
        private readonly PersonService _personService;

        [ObservableProperty]
        private string surName = string.Empty;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string? lastName;

        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;

        [ObservableProperty]
        private string? passportData;

        [ObservableProperty]
        private string? phone;

        [ObservableProperty]
        private string? status;

        public Action<bool>? CloseAction { get; set; }

        public AddPersonViewModel(PersonService personService)
        {
            _personService = personService;
        }

        [RelayCommand]
        private async Task Save()
        {
            var person = new Person
            {
                SurName = SurName,
                FirstName = FirstName,
                LastName = LastName,
                BirthDate = BirthDate,
                PassportData = PassportData,
                Phone = Phone,
                Status = Status
            };

            await _personService.CreateAsync(person);

            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
