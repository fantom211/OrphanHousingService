using OrphanHousingService.Models;

namespace OrphanHousingService.ViewModels.Details
{
    public class CommissionDecisionDetailsViewModel
    {
        public CommissionDecision Decision { get; }

        public CommissionDecisionDetailsViewModel(CommissionDecision decision)
        {
            Decision = decision;
        }
    }
}
