using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels.Details
{
    public partial class PersonDetailsViewModel
    {
        private readonly PersonService _personService;
        public Person Person { get; }

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = [];
        public Action<bool>? CloseAction { get; set; }
        public PersonDetailsViewModel (Person person, PersonService personService)
        {
            Person = person;
            _personService = personService;
            _ = Load(person.Id);
        }

        [RelayCommand]
        private async Task Load(Guid personId)
        {
            var family = await _personService.GetFamilyMembers(personId);

            FamilyMembers.Clear();

            foreach (var m in family)
                FamilyMembers.Add(m);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
