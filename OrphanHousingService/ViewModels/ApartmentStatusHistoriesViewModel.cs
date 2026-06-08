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
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ListCollectionManager<ApartmentStatusHistory> _listManager;

        public ICollectionView Histories => _listManager.View;

        [ObservableProperty]
        private ApartmentStatusHistory? selectedHistory;

        [ObservableProperty]
        private string? searchText;

        public ApartmentStatusHistoriesViewModel(
            ApartmentStatusHistoryService historyService,
            IServiceScopeFactory scopeFactory)
        {
            _historyService = historyService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<ApartmentStatusHistory>(h => new[]
            {
                h.Apartment?.Address,
                h.Basis,
                h.Comment
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "История квартир");
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
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddApartmentStatusHistoryViewModel>();
            var window = new AddApartmentStatusHistoryView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

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

            DetailWindowHelper.Show(
                new ApartmentStatusHistoryDetailsView(),
                new ApartmentStatusHistoryDetailsViewModel(SelectedHistory));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
