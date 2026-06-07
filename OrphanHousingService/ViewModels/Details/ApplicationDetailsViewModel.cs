using OrphanHousingService.Models;

namespace OrphanHousingService.ViewModels.Details
{
    public class ApplicationDetailsViewModel
    {
        public Application Application { get; }

        public ApplicationDetailsViewModel(Application application)
        {
            Application = application;
        }
    }
}
