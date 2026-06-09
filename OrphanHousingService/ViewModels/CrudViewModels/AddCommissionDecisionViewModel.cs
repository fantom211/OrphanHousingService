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
    public partial class AddCommissionDecisionViewModel : ObservableObject
    {
        private readonly CommissionDecisionService _commissionDecisionService;
        private readonly ApplicationService _applicationService;
        private readonly ContractWorkFlowService _workflowService;
        private Guid? _editId;
        private bool _isPrefilledFromApplication;

        public ObservableCollection<Application> Applications { get; } = [];

        public IReadOnlyList<EnumItem<DecisionType>> DecisionTypes { get; } =
            EnumHelper.GetItems<DecisionType>();

        public IReadOnlyList<EnumItem<DecisionResult>> Results { get; } =
            EnumHelper.GetItems<DecisionResult>();

        [ObservableProperty]
        private string windowTitle = "Добавить решение комиссии";

        [ObservableProperty]
        private Application? selectedApplication;

        [ObservableProperty]
        private DecisionType currentDecisionType;

        [ObservableProperty]
        private string decisionNumber = string.Empty;

        [ObservableProperty]
        private string suggestedDecisionNumber = string.Empty;

        [ObservableProperty]
        private DateTime decisionDate = DateTime.Today;

        [ObservableProperty]
        private DecisionResult currentResult;

        [ObservableProperty]
        private string? reason;

        [ObservableProperty]
        private string? comment;

        public bool IsEditMode => _editId.HasValue;
        public bool IsApplicationEditable => !IsEditMode && !_isPrefilledFromApplication;
        public bool IsDecisionTypeEditable => !IsEditMode && !_isPrefilledFromApplication;
        public bool IsResultEditable => !IsEditMode && !_isPrefilledFromApplication;

        public Action<bool>? CloseAction { get; set; }

        public AddCommissionDecisionViewModel(
            CommissionDecisionService commissionDecisionService,
            ApplicationService applicationService,
            ContractWorkFlowService workflowService)
        {
            _commissionDecisionService = commissionDecisionService;
            _applicationService = applicationService;
            _workflowService = workflowService;
            SuggestedDecisionNumber = _commissionDecisionService.GenerateNumber();
        }

        public async Task PrepareStandaloneAsync()
        {
            await LoadApplicationsAsync();
        }

        public async Task InitializeForEditAsync(CommissionDecision decision)
        {
            _editId = decision.Id;
            WindowTitle = "Редактировать решение комиссии";
            DecisionNumber = decision.DecisionNumber;
            DecisionDate = decision.DecisionDate;
            Reason = decision.Reason;
            Comment = decision.Comment;
            NotifyEditabilityChanged();
            await LoadApplicationsAsync();
            SelectedApplication = EntityComboHelper.FindById(Applications, decision.ApplicationId);
        }

        public async Task InitializeForApplicationAsync(Application application, DecisionResult result)
        {
            _isPrefilledFromApplication = true;
            WindowTitle = result == DecisionResult.Approved
                ? "Одобрить заявление"
                : "Отклонить заявление";
            CurrentResult = result;
            CurrentDecisionType = MapApplicationType(application.ApplicationType, result);
            DecisionDate = DateTime.Today;
            Comment = application.Comment;
            SelectedApplication = application;
            NotifyEditabilityChanged();
            await LoadApplicationsAsync();
            SelectedApplication = EntityComboHelper.FindById(Applications, application.Id) ?? application;
        }

        private void NotifyEditabilityChanged()
        {
            OnPropertyChanged(nameof(IsApplicationEditable));
            OnPropertyChanged(nameof(IsDecisionTypeEditable));
            OnPropertyChanged(nameof(IsResultEditable));
        }

        private static DecisionType MapApplicationType(ApplicationType type, DecisionResult result)
        {
            if (result == DecisionResult.Rejected)
                return DecisionType.Refusal;

            return type switch
            {
                ApplicationType.ContractReduction => DecisionType.ContractReduction,
                ApplicationType.SocialRentTransfer => DecisionType.SocialRentTransfer,
                ApplicationType.Extension => DecisionType.Extension,
                _ => DecisionType.Refusal
            };
        }

        private async Task LoadApplicationsAsync()
        {
            var items = await _applicationService.GetAllAsync();
            Applications.Clear();
            foreach (var application in items)
                Applications.Add(application);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (SelectedApplication == null)
                {
                    ValidationDialogHelper.ShowError(
                        new Exception("Выберите заявление."));
                    return;
                }

                if (IsEditMode)
                {
                    var entity = new CommissionDecision
                    {
                        Id = _editId!.Value,
                        DecisionNumber = string.IsNullOrWhiteSpace(DecisionNumber)
                            ? SuggestedDecisionNumber
                            : DecisionNumber,
                        DecisionDate = DecisionDate.Date,
                        Reason = Reason,
                        Comment = Comment
                    };

                    await _commissionDecisionService.UpdateAsync(entity);
                }
                else
                {
                    var entity = new CommissionDecision
                    {
                        ApplicationId = SelectedApplication.Id,
                        DecisionType = CurrentDecisionType,
                        DecisionNumber = string.IsNullOrWhiteSpace(DecisionNumber)
                            ? SuggestedDecisionNumber
                            : DecisionNumber,
                        DecisionDate = DecisionDate.Date,
                        Result = CurrentResult,
                        Reason = Reason,
                        Comment = Comment
                    };

                    await _commissionDecisionService.CreateAsync(entity);
                    await _workflowService.RegisterDecisionAsync(entity);
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
