using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.ViewModels.Interfaces;
using System;

namespace OrphanHousingService.ViewModels
{
    public partial class MainViewModel : ObservableObject, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private IServiceScope? _viewScope;

        [ObservableProperty]
        private ICrudViewModel? currentViewModel;

        public MainViewModel(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Initialize()
        {
            OpenView<ContractsViewModel>();
        }

        [RelayCommand]
        private void OpenContracts() => OpenView<ContractsViewModel>();

        [RelayCommand]
        private void OpenPeople() => OpenView<PeopleViewModel>();

        [RelayCommand]
        private void OpenApartments() => OpenView<ApartmentsViewModel>();

        [RelayCommand]
        private void OpenApartmentStatusHistory() => OpenView<ApartmentStatusHistoriesViewModel>();

        [RelayCommand]
        private void OpenApplications() => OpenView<ApplicationsViewModel>();

        [RelayCommand]
        private void OpenCommissionDecisions() => OpenView<CommissionDecisionsViewModel>();

        [RelayCommand]
        private void OpenUtilityDebts() => OpenView<UtilityDebtsViewModel>();

        [RelayCommand]
        private void OpenFamilyMembers() => OpenView<FamilyMembersViewModel>();

        public void NavigateToPerson(Guid personId)
        {
            var vm = OpenView<PeopleViewModel>();
            vm.SelectById(personId);
        }

        public void NavigateToApartment(Guid apartmentId)
        {
            var vm = OpenView<ApartmentsViewModel>();
            vm.SelectById(apartmentId);
        }

        private T OpenView<T>() where T : class, ICrudViewModel
        {
            _viewScope?.Dispose();
            _viewScope = _scopeFactory.CreateScope();
            var vm = _viewScope.ServiceProvider.GetRequiredService<T>();
            CurrentViewModel = vm;
            return vm;
        }

        public void Dispose()
        {
            _viewScope?.Dispose();
        }
    }
}
