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
    public partial class AddCommissionDecisionViewModel : ObservableObject
    {
        private readonly CommissionDecisionService _commissionDecisionService;
        private readonly ApplicationService _applicationService;
        private readonly ContractWorkFlowService _workflowService;
        private Guid? _editId;

        [ObservableProperty]
        private ObservableCollection<Application> applications = [];

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
        public bool IsApplicationEditable => !IsEditMode;

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
            _ = LoadAsync();
        }

        public void InitializeForEdit(CommissionDecision decision)
        {
            _editId = decision.Id;
            WindowTitle = "Редактировать решение комиссии";
            DecisionNumber = decision.DecisionNumber;
            DecisionDate = decision.DecisionDate;
            Reason = decision.Reason;
            Comment = decision.Comment;
            OnPropertyChanged(nameof(IsApplicationEditable));
            _ = ApplyApplicationAsync(decision.ApplicationId);
        }

        private async Task ApplyApplicationAsync(Guid applicationId)
        {
            if (Applications.Count == 0)
                await LoadAsync();

            SelectedApplication = Applications.FirstOrDefault(a => a.Id == applicationId);
        }

        private async Task LoadAsync()
        {
            var items = await _applicationService.GetAllAsync();

            Applications.Clear();
            foreach (var a in items)
                Applications.Add(a);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (IsEditMode)
                {
                    var entity = new CommissionDecision
                    {
                        Id = _editId!.Value,
                        DecisionNumber = string.IsNullOrWhiteSpace(DecisionNumber)
                            ? SuggestedDecisionNumber
                            : DecisionNumber,
                        DecisionDate = DecisionDate,
                        Reason = Reason,
                        Comment = Comment
                    };

                    await _commissionDecisionService.UpdateAsync(entity);
                }
                else
                {
                    var entity = new CommissionDecision
                    {
                        ApplicationId = SelectedApplication!.Id,
                        DecisionType = CurrentDecisionType,
                        DecisionNumber = string.IsNullOrWhiteSpace(DecisionNumber)
                            ? SuggestedDecisionNumber
                            : DecisionNumber,
                        DecisionDate = DecisionDate,
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
