using System;

namespace OrphanHousingService.ViewModels.Interfaces
{
    public interface ISelectableViewModel
    {
        void SelectById(Guid id);
    }
}
