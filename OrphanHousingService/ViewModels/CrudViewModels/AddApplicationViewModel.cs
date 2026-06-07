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
    public partial class AddApplicationViewModel : ObservableObject
    {
        private readonly ContractService _contractService;
        private readonly ApplicationService _applicationService;

        public IReadOnlyList<EnumItem<ApplicationType>> ApplicationTypes { get; } =
            EnumHelper.GetItems<ApplicationType>();

        public IReadOnlyList<EnumItem<ApplicationStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApplicationStatus>();

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private Contract? selectedContract;

        [ObservableProperty]
        private ApplicationType currentApplicationType;

        [ObservableProperty]
        private DateTime applicationDate = DateTime.Today;

        [ObservableProperty]
        private ApplicationStatus currentStatus;

        [ObservableProperty]
        private string? comment;

        [ObservableProperty]
        private string? applicationNumber;

        public Action<bool>? CloseAction { get; set; }

        public AddApplicationViewModel(ApplicationService applicationService, ContractService contractService)
        {
            _contractService = contractService;
            _applicationService = applicationService;
            _ = LoadAsync();
        }

        private async Task LoadAsync()
        {
            var contracts = await _contractService.GetAllAsync();

            foreach (var c in contracts)
                Contracts.Add(c);
        }

        [RelayCommand]
        private async Task Save()
        {
            var entity = new Application
            {
                ContractId = SelectedContract!.Id,
                ApplicationNumber = ApplicationNumber,
                ApplicationType = CurrentApplicationType,
                ApplicationDate = ApplicationDate,
                Status = CurrentStatus,
                Comment = Comment
            };

            await _applicationService.CreateAsync(entity);

            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
