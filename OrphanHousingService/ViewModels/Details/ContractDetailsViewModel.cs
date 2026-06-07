using OrphanHousingService.Models;

namespace OrphanHousingService.ViewModels.Details
{
    public class ContractDetailsViewModel
    {
        public Contract Contract { get; }

        public ContractDetailsViewModel(Contract contract)
        {
            Contract = contract;
        }
    }
}
