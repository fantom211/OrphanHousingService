using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models.Enums;
using ApplicationModel = OrphanHousingService.Models.Application;
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
    public partial class ApplicationsViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly ApplicationService _applicationService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ListCollectionManager<ApplicationModel> _listManager;

        public ICollectionView Applications => _listManager.View;

        [ObservableProperty]
        private ApplicationModel? selectedApplication;

        [ObservableProperty]
        private string? searchText;

        public bool CanProcessApplication =>
            SelectedApplication?.Status == ApplicationStatus.Pending;

        public ApplicationsViewModel(
            ApplicationService applicationService,
            IServiceScopeFactory scopeFactory)
        {
            _applicationService = applicationService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<ApplicationModel>(a => new[]
            {
                a.ApplicationNumber,
                a.Contract?.ContractNumber,
                a.Contract?.Person?.FullName,
                a.Contract?.Apartment?.Address,
                a.Comment
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Заявления");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        partial void OnSelectedApplicationChanged(ApplicationModel? value) =>
            RefreshProcessCommands();

        public async Task LoadAsync()
        {
            var selectedId = SelectedApplication?.Id;
            var items = await _applicationService.GetAllAsync();
            _listManager.SetItems(items);

            if (selectedId.HasValue)
                SelectedApplication = Applications.Cast<ApplicationModel>().FirstOrDefault(a => a.Id == selectedId.Value);

            RefreshProcessCommands();
        }

        private void RefreshProcessCommands()
        {
            OnPropertyChanged(nameof(CanProcessApplication));
            ApproveApplicationCommand.NotifyCanExecuteChanged();
            RejectApplicationCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddApplicationViewModel>();
            var window = new AddApplicationView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedApplication == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddApplicationViewModel>();
            vm.InitializeForEdit(SelectedApplication);

            var window = new AddApplicationView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedApplication == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedApplication.ApplicationNumber ?? "заявление"))
                return;

            try
            {
                await _applicationService.DeleteAsync(SelectedApplication.Id);
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
            if (SelectedApplication == null)
                return;

            DetailWindowHelper.Show(
                new ApplicationDetailsView(),
                new ApplicationDetailsViewModel(SelectedApplication));
        }

        [RelayCommand(CanExecute = nameof(CanProcessApplication))]
        private async void ApproveApplication()
        {
            if (SelectedApplication == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddCommissionDecisionViewModel>();
            await vm.InitializeForApplicationAsync(SelectedApplication, DecisionResult.Approved);

            var window = new AddCommissionDecisionView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await ReloadAfterProcessAsync();
        }

        [RelayCommand(CanExecute = nameof(CanProcessApplication))]
        private async void RejectApplication()
        {
            if (SelectedApplication == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddCommissionDecisionViewModel>();
            await vm.InitializeForApplicationAsync(SelectedApplication, DecisionResult.Rejected);

            var window = new AddCommissionDecisionView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await ReloadAfterProcessAsync();
        }

        private async Task ReloadAfterProcessAsync()
        {
            await LoadAsync();
            RefreshProcessCommands();
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
