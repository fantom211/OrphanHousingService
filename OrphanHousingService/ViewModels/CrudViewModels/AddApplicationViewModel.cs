using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
using System;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddApplicationViewModel : ObservableObject
    {
        private readonly ContractService _contractService;
        private readonly ApplicationService _applicationService;
        private Guid? _editId;

        public IReadOnlyList<EnumItem<ApplicationType>> ApplicationTypes { get; } =
            EnumHelper.GetItems<ApplicationType>();

        public IReadOnlyList<EnumItem<ApplicationStatus>> Statuses { get; } =
            EnumHelper.GetItems<ApplicationStatus>();

        public ObservableCollection<Contract> Contracts { get; } = [];

        [ObservableProperty]
        private string windowTitle = "Добавить заявление";

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

        [ObservableProperty]
        private string suggestedApplicationNumber = string.Empty;

        public bool IsEditMode => _editId.HasValue;
        public bool IsContractEditable => !IsEditMode;

        public Action<bool>? CloseAction { get; set; }

        public AddApplicationViewModel(ApplicationService applicationService, ContractService contractService)
        {
            _contractService = contractService;
            _applicationService = applicationService;
            SuggestedApplicationNumber = _applicationService.GenerateNumber();
            _ = LoadAsync();
        }

        public void InitializeForEdit(Application application)
        {
            _editId = application.Id;
            WindowTitle = "Редактировать заявление";
            ApplicationNumber = application.ApplicationNumber;
            ApplicationDate = application.ApplicationDate;
            Comment = application.Comment;
            OnPropertyChanged(nameof(IsContractEditable));
            _ = ApplyContractAsync(application.ContractId);
        }

        private async Task ApplyContractAsync(Guid contractId)
        {
            if (Contracts.Count == 0)
                await LoadAsync();

            SelectedContract = Contracts.FirstOrDefault(c => c.Id == contractId);
        }

        private async Task LoadAsync()
        {
            var contracts = await _contractService.GetAllAsync();

            Contracts.Clear();
            foreach (var c in contracts)
                Contracts.Add(c);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (IsEditMode)
                {
                    var entity = new Application
                    {
                        Id = _editId!.Value,
                        ApplicationNumber = string.IsNullOrWhiteSpace(ApplicationNumber)
                            ? SuggestedApplicationNumber
                            : ApplicationNumber,
                        ApplicationDate = ApplicationDate,
                        Comment = Comment
                    };

                    await _applicationService.UpdateAsync(entity);
                }
                else
                {
                    var entity = new Application
                    {
                        ContractId = SelectedContract!.Id,
                        ApplicationNumber = string.IsNullOrWhiteSpace(ApplicationNumber)
                            ? SuggestedApplicationNumber
                            : ApplicationNumber,
                        ApplicationType = CurrentApplicationType,
                        ApplicationDate = ApplicationDate,
                        Status = CurrentStatus,
                        Comment = Comment
                    };

                    await _applicationService.CreateAsync(entity);
                }

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
