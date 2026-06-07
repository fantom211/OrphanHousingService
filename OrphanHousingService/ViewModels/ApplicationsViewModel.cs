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
    public partial class ApplicationsViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly ApplicationService _applicationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ListCollectionManager<Application> _listManager;

        public ICollectionView Applications => _listManager.View;

        [ObservableProperty]
        private Application? selectedApplication;

        [ObservableProperty]
        private string? searchText;

        public ApplicationsViewModel(ApplicationService applicationService,
                                    IServiceProvider serviceProvider)
        {
            _applicationService = applicationService;
            _serviceProvider = serviceProvider;
            _listManager = new ListCollectionManager<Application>(a => new[]
            {
                a.ApplicationNumber,
                a.Contract?.ContractNumber,
                a.Contract?.Person?.FullName,
                a.Contract?.Apartment?.Address,
                a.Comment
            });
            _ = LoadAsync();
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var items = await _applicationService.GetAllAsync();
            _listManager.SetItems(items);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddApplicationView>();

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedApplication == null)
                return;

            var vm = _serviceProvider.GetRequiredService<AddApplicationViewModel>();
            vm.InitializeForEdit(SelectedApplication);

            var window = new AddApplicationView(vm);

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

            var window = _serviceProvider.GetRequiredService<ApplicationDetailsView>();
            DetailWindowHelper.Show(window, new ApplicationDetailsViewModel(SelectedApplication));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
