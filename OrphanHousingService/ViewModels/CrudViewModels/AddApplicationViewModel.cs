using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Business;
using OrphanHousingService.Services.Helpers;
using OrphanHousingService.ViewModels.Helpers;
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

        public AddApplicationViewModel(
            ApplicationService applicationService,
            ContractService contractService)
        {
            _contractService = contractService;
            _applicationService = applicationService;
            SuggestedApplicationNumber = _applicationService.GenerateNumber();
        }

        public async Task PrepareAsync()
        {
            await LoadContractsAsync();
        }

        public async Task InitializeForEditAsync(Application application)
        {
            _editId = application.Id;
            WindowTitle = "Редактировать заявление";
            ApplicationNumber = application.ApplicationNumber;
            ApplicationDate = application.ApplicationDate;
            CurrentApplicationType = application.ApplicationType;
            CurrentStatus = application.Status;
            Comment = application.Comment;
            OnPropertyChanged(nameof(IsContractEditable));
            await LoadContractsAsync();
            SelectedContract = EntityComboHelper.FindById(Contracts, application.ContractId);
        }

        private async Task LoadContractsAsync()
        {
            var contracts = await _contractService.GetAllAsync();
            Contracts.Clear();
            foreach (var contract in contracts)
                Contracts.Add(contract);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (IsEditMode)
                {
                    await _applicationService.UpdateAsync(new Application
                    {
                        Id = _editId!.Value,
                        ApplicationNumber = string.IsNullOrWhiteSpace(ApplicationNumber)
                            ? SuggestedApplicationNumber
                            : ApplicationNumber,
                        ApplicationDate = ApplicationDate,
                        Comment = Comment
                    });
                }
                else
                {
                    await _applicationService.CreateAsync(new Application
                    {
                        ContractId = SelectedContract!.Id,
                        ApplicationNumber = string.IsNullOrWhiteSpace(ApplicationNumber)
                            ? SuggestedApplicationNumber
                            : ApplicationNumber,
                        ApplicationType = CurrentApplicationType,
                        ApplicationDate = ApplicationDate,
                        Status = CurrentStatus,
                        Comment = Comment
                    });
                }

                CloseAction?.Invoke(true);
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void Cancel() => CloseAction?.Invoke(false);
    }
}
