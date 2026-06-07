using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.CrudViewModels;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels
{
    public partial class ApartmentStatusHistoriesViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly ApartmentStatusHistoryService _historyService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ListCollectionManager<ApartmentStatusHistory> _listManager;

        public ICollectionView Histories => _listManager.View;

        [ObservableProperty]
        private ApartmentStatusHistory? selectedHistory;

        [ObservableProperty]
        private string? searchText;

        public ApartmentStatusHistoriesViewModel(
            ApartmentStatusHistoryService historyService,
            IServiceProvider serviceProvider)
        {
            _historyService = historyService;
            _serviceProvider = serviceProvider;
            _listManager = new ListCollectionManager<ApartmentStatusHistory>(h => new[]
            {
                h.Apartment?.Address,
                h.Basis,
                h.Comment
            });
            _ = LoadAsync();
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var histories = await _historyService.GetAllAsync();
            _listManager.SetItems(histories);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApartmentStatusHistoryView>();

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private void Edit()
        {
            ValidationDialogHelper.ShowError(
                new InvalidOperationException("Записи истории нельзя редактировать"));
        }

        [RelayCommand]
        private void Delete()
        {
            ValidationDialogHelper.ShowError(
                new InvalidOperationException("Записи истории нельзя удалять"));
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedHistory == null)
                return;

            var window = _serviceProvider.GetRequiredService<ApartmentStatusHistoryDetailsView>();
            DetailWindowHelper.Show(window, new ApartmentStatusHistoryDetailsViewModel(SelectedHistory));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
