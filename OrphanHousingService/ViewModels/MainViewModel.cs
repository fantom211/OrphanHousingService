using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
