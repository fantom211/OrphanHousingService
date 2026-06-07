using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ContractWorkFlowService
    {
        private readonly ApplicationService _applicationService;
        private readonly ContractService _contractService;
        private readonly CommissionDecisionService _decisionService;

        public ContractWorkFlowService(
            ApplicationService applicationService,
            ContractService contractService,
            CommissionDecisionService decisionService)
        {
            _applicationService = applicationService;
            _contractService = contractService;
            _decisionService = decisionService;
        }

        public async Task ProcessingDecisionAsync(CommissionDecision decision)
        {
            var application = await _applicationService.GetByIdAsync(decision.ApplicationId);

            if (application is null) throw new InvalidOperationException("Заявка не найдена.");

            switch (decision.Result)
            {
                case DecisionResult.Approved:
                    await ProcessApprovedAsync(application, decision);
                    break;

                case DecisionResult.Rejected:
                    application.Status = ApplicationStatus.Rejected;
                    await _applicationService.RejectAsync(application.Id, decision.Reason!);
                    break;
            }
        }

        private async Task ProcessApprovedAsync(
            Application application,
            CommissionDecision decision)
        {
            application.Status = ApplicationStatus.Approved;

            await _applicationService.ApproveAsync(application.Id);

            switch (application.ApplicationType)
            {
                case ApplicationType.ContractReduction:
                    await _contractService.ShortenContractEndDate(application.ContractId);
                    break;

                case ApplicationType.SocialRentTransfer:
                    await _contractService.TransferToSocialRent(application.ContractId);
                    break;

                case ApplicationType.Extension:
                    await _contractService.ExtendAsync(application.ContractId);
                    break;
            }
        }
        public async Task RegisterDecisionAsync(
            CommissionDecision decision)
        {
            await ProcessingDecisionAsync(decision);
        }

    }
}
