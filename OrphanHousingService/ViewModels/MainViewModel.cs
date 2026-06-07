using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views;
using System;

namespace OrphanHousingService.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;
        [ObservableProperty]
        private ICrudViewModel currentViewModel;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            //CurrentViewModel = _serviceProvider.GetRequiredService<ContractsViewModel>();

        }

        public void Initialize()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<ContractsViewModel>();
        }

        [RelayCommand]
        private void OpenContracts()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<ContractsViewModel>();
        }

        [RelayCommand]
        private void OpenPeople()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<PeopleViewModel>();
        }

        [RelayCommand]
        private void OpenApartments()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<ApartmentsViewModel>();
        }

        [RelayCommand]
        private void OpenApartmentStatusHistory()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<ApartmentStatusHistoriesViewModel>();
        }

        [RelayCommand]
        private void OpenApplications()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<ApplicationsViewModel>();
        }

        [RelayCommand]
        private void OpenCommissionDecisions()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<CommissionDecisionsViewModel>();
        }

        [RelayCommand]
        private void OpenUtilityDebts()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<UtilityDebtsViewModel>();
        }

        [RelayCommand]
        private void OpenFamilyMembers()
        {
            CurrentViewModel = _serviceProvider.GetRequiredService<FamilyMembersViewModel>();
        }

        public void NavigateToPerson(Guid personId)
        {
            var vm = _serviceProvider.GetRequiredService<PeopleViewModel>();
            CurrentViewModel = vm;
            vm.SelectById(personId);
        }

        public void NavigateToApartment(Guid apartmentId)
        {
            var vm = _serviceProvider.GetRequiredService<ApartmentsViewModel>();
            CurrentViewModel = vm;
            vm.SelectById(apartmentId);
        }
    }
}
