using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels
{
    public partial class PeopleViewModel : ObservableObject, ICrudViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PersonService _personService;
        public ObservableCollection<Person> People { get; } = [];

        [ObservableProperty]
        private Person? selectedPerson;

        public PeopleViewModel(
             PersonService personService,
             IServiceProvider serviceProvider)
        {
            _personService = personService;
            _serviceProvider = serviceProvider;

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            People.Clear();

            var people = await _personService.GetAllAsync();

            foreach (var person in people)
                People.Add(person);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddPersonView>();

            if (window.ShowDialog() == true)
            {
                People.Clear();
                await LoadAsync();
            }
        }

        [RelayCommand]
        private void Edit()
        {

        }

        [RelayCommand]
        private void Delete()
        {

        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
