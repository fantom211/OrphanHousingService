using OrphanHousingService.Models;

namespace OrphanHousingService.ViewModels.Details
{
    public class UtilityDebtDetailsViewModel
    {
        public UtilityDebt UtilityDebt { get; }

        public UtilityDebtDetailsViewModel(UtilityDebt utilityDebt)
        {
            UtilityDebt = utilityDebt;
        }
    }
}
