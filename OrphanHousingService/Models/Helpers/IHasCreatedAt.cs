using System;

namespace OrphanHousingService.Models.Helpers
{
    public interface IHasCreatedAt
    {
        DateTime CreatedAt { get; set; }
    }
}
