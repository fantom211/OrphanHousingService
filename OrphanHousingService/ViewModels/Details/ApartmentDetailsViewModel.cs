using CommunityToolkit.Mvvm.ComponentModel;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using System.Collections.ObjectModel;
using System.Linq;

namespace OrphanHousingService.ViewModels.Details
{
    public partial class ApartmentDetailsViewModel : ObservableObject
    {
        private readonly ApartmentStatusHistoryService _historyService;

        [ObservableProperty]
        private Apartment apartment = null!;

        public ObservableCollection<ApartmentStatusHistory> History { get; } = [];

        public ApartmentDetailsViewModel(
            Apartment apartment,
            ApartmentStatusHistoryService historyService)
        {
            _historyService = historyService;
            Apartment = apartment;
            _ = LoadHistoryAsync();
        }

        private async Task LoadHistoryAsync()
        {
            var items = await _historyService.GetAllAsync();
            var filtered = items
                .Where(h => h.ApartmentId == Apartment.Id)
                .OrderByDescending(h => h.ChangeDate);

            History.Clear();
            foreach (var item in filtered)
                History.Add(item);
        }
    }
}
