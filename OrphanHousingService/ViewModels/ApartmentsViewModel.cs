using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.CrudViewModels;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.ComponentModel;
using System.Windows.Data;

namespace OrphanHousingService.ViewModels
{
    public partial class ApartmentsViewModel : ObservableObject, ISearchableListViewModel, ISelectableViewModel
    {
        private readonly ApartmentService _apartmentService;
        private readonly ApartmentStatusHistoryService _historyService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ListCollectionManager<Apartment> _listManager;
        private Guid? _pendingSelectionId;

        public ICollectionView Apartments => _listManager.View;

        [ObservableProperty]
        private Apartment? selectedApartment;

        [ObservableProperty]
        private string? searchText;

        public ApartmentsViewModel(
            ApartmentService apartmentService,
            ApartmentStatusHistoryService historyService,
            IServiceScopeFactory scopeFactory)
        {
            _apartmentService = apartmentService;
            _historyService = historyService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<Apartment>(a => new[]
            {
                a.Address,
                a.CadastralNumber
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Квартиры");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var apartments = await _apartmentService.GetAllAsync();
            _listManager.SetItems(apartments);

            if (_pendingSelectionId.HasValue)
            {
                SelectById(_pendingSelectionId.Value);
                _pendingSelectionId = null;
            }
        }

        public void SelectById(Guid id)
        {
            SelectedApartment = Apartments.Cast<Apartment>().FirstOrDefault(a => a.Id == id);

            if (SelectedApartment == null)
                _pendingSelectionId = id;
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddApartmentViewModel>();
            var window = new AddApartmentView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedApartment == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddApartmentViewModel>();
            vm.InitializeForEdit(SelectedApartment);

            var window = new AddApartmentView(vm);

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedApartment == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedApartment.Address))
                return;

            try
            {
                await _apartmentService.DeleteAsync(SelectedApartment.Id);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedApartment == null)
                return;

            DetailWindowHelper.Show(
                new ApartmentDetailsView(),
                new ApartmentDetailsViewModel(SelectedApartment, _historyService));
        }

        [RelayCommand]
        private async void AddContract()
        {
            if (SelectedApartment == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddContractViewModel>();
            vm.InitializeForApartment(SelectedApartment.Id);

            var window = new AddContractView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void TransferToSocialRent()
        {
            if (SelectedApartment == null)
                return;

            if (!TryGetReason("Перевод в соц. найм", out var reason))
                return;

            try
            {
                await _apartmentService.TransferToSocialRentAsync(SelectedApartment.Id, reason);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private async void TransferToSpecialFund()
        {
            if (SelectedApartment == null)
                return;

            if (!TryGetReason("Перевод в спец. жилой фонд", out var reason))
                return;

            try
            {
                await _apartmentService.TransferToSpecialFundAsync(SelectedApartment.Id, reason);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        [RelayCommand]
        private async void Privatize()
        {
            if (SelectedApartment == null)
                return;

            if (!TryGetReason("Приватизация", out var reason))
                return;

            try
            {
                await _apartmentService.PrivatizeAsync(SelectedApartment.Id, reason);
                await LoadAsync();
            }
            catch (Exception ex)
            {
                ValidationDialogHelper.ShowError(ex);
            }
        }

        private bool TryGetReason(string title, out string? reason)
        {
            var vm = new ReasonInputViewModel
            {
                Title = title,
                Prompt = "Причина / основание (необязательно):"
            };

            var window = new ReasonInputView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() != true)
            {
                reason = null;
                return false;
            }

            reason = vm.Reason;
            return true;
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
