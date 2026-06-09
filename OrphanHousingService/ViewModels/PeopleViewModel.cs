using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.CrudViewModels;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels
{
    public partial class PeopleViewModel : ObservableObject, ISearchableListViewModel, ISelectableViewModel
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly PersonService _personService;
        private readonly ListCollectionManager<Person> _listManager;
        private Guid? _pendingSelectionId;

        public ICollectionView People => _listManager.View;

        [ObservableProperty]
        private Person? selectedPerson;

        [ObservableProperty]
        private string? searchText;

        public PeopleViewModel(
             PersonService personService,
             IServiceScopeFactory scopeFactory)
        {
            _personService = personService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<Person>(p => new[]
            {
                p.FullName,
                p.PassportData,
                p.Phone
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Люди");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var selectedId = SelectedPerson?.Id;
            var people = await _personService.GetCitizensAsync();
            _listManager.SetItems(people);

            SelectedPerson = _listManager.RestoreSelection(selectedId, p => p.Id);

            if (_pendingSelectionId.HasValue)
            {
                SelectById(_pendingSelectionId.Value);
                _pendingSelectionId = null;
            }
        }

        public void SelectById(Guid id)
        {
            SelectedPerson = People.Cast<Person>().FirstOrDefault(p => p.Id == id);

            if (SelectedPerson == null)
                _pendingSelectionId = id;
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddPersonViewModel>();
            var window = new AddPersonView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedPerson == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddPersonViewModel>();
            vm.InitializeForEdit(SelectedPerson);

            var window = new AddPersonView(vm);

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedPerson == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedPerson.FullName))
                return;

            try
            {
                await _personService.DeleteAsync(SelectedPerson.Id);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedPerson == null)
                return;

            DetailWindowHelper.Show(
                new PersonDetailsView(),
                new PersonDetailsViewModel(SelectedPerson, _personService));
        }

        [RelayCommand]
        private async void AddContract()
        {
            if (SelectedPerson == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddContractViewModel>();
            await vm.InitializeForPersonAsync(SelectedPerson.Id);

            var window = new AddContractView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void AddFamilyMember()
        {
            if (SelectedPerson == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddFamilyMemberViewModel>();
            await vm.InitializeForPersonAsync(SelectedPerson.Id);
            if (vm.InitializationFailed)
                return;

            var window = new AddFamilyMemberView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
