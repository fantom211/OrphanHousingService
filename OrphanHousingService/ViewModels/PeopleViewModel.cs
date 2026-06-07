using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels
{
    public partial class PeopleViewModel : ObservableObject, ICrudViewModel, ISelectableViewModel
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PersonService _personService;
        private Guid? _pendingSelectionId;

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

            if (_pendingSelectionId.HasValue)
            {
                SelectById(_pendingSelectionId.Value);
                _pendingSelectionId = null;
            }
        }

        public void SelectById(Guid id)
        {
            SelectedPerson = People.FirstOrDefault(p => p.Id == id);

            if (SelectedPerson == null)
                _pendingSelectionId = id;
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddPersonView>();

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private void Edit()
        {
        }

        [RelayCommand]
        private void Delete()
        {
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedPerson == null)
                return;

            var window = _serviceProvider.GetRequiredService<PersonDetailsView>();
            DetailWindowHelper.Show(window, new PersonDetailsViewModel(SelectedPerson, _personService));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
