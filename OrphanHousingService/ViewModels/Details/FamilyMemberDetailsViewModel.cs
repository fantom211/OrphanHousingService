using OrphanHousingService.Models;

namespace OrphanHousingService.ViewModels.Details
{
    public class FamilyMemberDetailsViewModel
    {
        public FamilyMember FamilyMember { get; }

        public FamilyMemberDetailsViewModel(FamilyMember familyMember)
        {
            FamilyMember = familyMember;
        }
    }
}
