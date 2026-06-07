namespace OrphanHousingService.ViewModels.Interfaces
{
    public interface ISearchableListViewModel : ICrudViewModel
    {
        string? SearchText { get; set; }
    }
}
