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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract = OrphanHousingService.Models.Application;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class AddCommissionDecisionViewModel : ObservableObject
    {
        private readonly CommissionDecisionService _commissionDecisionService;
        private readonly ApplicationService _applicationService;
        private readonly ContractWorkFlowService _workflowService;
        [ObservableProperty]
        private ObservableCollection<Application> applications = [];

        public IReadOnlyList<EnumItem<DecisionType>> DecisionTypes { get; } =
            EnumHelper.GetItems<DecisionType>();

        public IReadOnlyList<EnumItem<DecisionResult>> Results { get; } =
            EnumHelper.GetItems<DecisionResult>();

        [ObservableProperty]
        private Application? selectedApplication;


        [ObservableProperty]
        private DecisionType currentDecisionType;

        [ObservableProperty]
        private string decisionNumber = string.Empty;

        [ObservableProperty]
        private DateTime decisionDate = DateTime.Today;

        [ObservableProperty]
        private DecisionResult currentResult;

        [ObservableProperty]
        private string? reason;

        [ObservableProperty]
        private string? comment;

        public Action<bool>? CloseAction { get; set; }

        public AddCommissionDecisionViewModel(
            CommissionDecisionService commissionDecisionService, ApplicationService applicationService, ContractWorkFlowService workflowService)
        {
            _commissionDecisionService = commissionDecisionService;
            _applicationService = applicationService;
            _workflowService = workflowService;

            _ = LoadAsync();
            
        }
        private async Task LoadAsync()
        {
            var applications = await _applicationService.GetAllAsync();

            foreach (var a in applications)
                Applications.Add(a);
        }


        [RelayCommand]
        private async Task Save()
        {
            var entity = new CommissionDecision
            {
                ApplicationId = SelectedApplication!.Id,
                DecisionType = CurrentDecisionType,
                DecisionNumber = DecisionNumber,
                DecisionDate = DecisionDate,
                Result = CurrentResult,
                Reason = Reason,
                Comment = Comment
            };

            await _commissionDecisionService.CreateAsync(entity);

            await _workflowService.RegisterDecisionAsync(entity);

            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
