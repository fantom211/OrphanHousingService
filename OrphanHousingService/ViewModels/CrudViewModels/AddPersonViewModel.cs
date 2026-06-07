using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
using System;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddPersonViewModel : ObservableObject
    {
        private readonly PersonService _personService;
        private Guid? _editId;

        [ObservableProperty]
        private string windowTitle = "Добавить человека";

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

        public bool IsEditMode => _editId.HasValue;

        public Action<bool>? CloseAction { get; set; }

        public AddPersonViewModel(PersonService personService)
        {
            _personService = personService;
        }

        public void InitializeForEdit(Person person)
        {
            _editId = person.Id;
            WindowTitle = "Редактировать человека";
            SurName = person.SurName;
            FirstName = person.FirstName;
            LastName = person.LastName;
            BirthDate = person.BirthDate;
            PassportData = person.PassportData;
            Phone = person.Phone;
            Status = person.Status;
        }

        [RelayCommand]
        private async Task Save()
        {
            if (!PassportFormatHelper.IsValid(PassportData))
            {
                ValidationDialogHelper.ShowError(
                    new Exception(PassportFormatHelper.FormatDescription));
                return;
            }

            try
            {
                var person = new Person
                {
                    Id = _editId ?? Guid.Empty,
                    SurName = SurName,
                    FirstName = FirstName,
                    LastName = LastName,
                    BirthDate = BirthDate,
                    PassportData = PassportData,
                    Phone = Phone,
                    Status = Status
                };

                if (IsEditMode)
                    await _personService.UpdateAsync(person);
                else
                    await _personService.CreateAsync(person);

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
