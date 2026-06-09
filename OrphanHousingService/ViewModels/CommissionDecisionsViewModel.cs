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
    public partial class CommissionDecisionsViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly CommissionDecisionService _decisionService;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ListCollectionManager<CommissionDecision> _listManager;

        public ICollectionView CommissionDecisions => _listManager.View;

        [ObservableProperty]
        private CommissionDecision? selectedCommissionDecision;

        [ObservableProperty]
        private string? searchText;

        public CommissionDecisionsViewModel(
            CommissionDecisionService decisionService,
            IServiceScopeFactory scopeFactory)
        {
            _decisionService = decisionService;
            _scopeFactory = scopeFactory;
            _listManager = new ListCollectionManager<CommissionDecision>(d => new[]
            {
                d.DecisionNumber,
                d.Application?.ApplicationNumber,
                d.Application?.Contract?.ContractNumber,
                d.Application?.Contract?.Person?.FullName,
                d.Reason,
                d.Comment
            });
            _ = ViewModelLoadHelper.RunSafeAsync(LoadAsync, "Решения комиссии");
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var selectedId = SelectedCommissionDecision?.Id;
            var items = await _decisionService.GetAllAsync();
            _listManager.SetItems(items);
            SelectedCommissionDecision = _listManager.RestoreSelection(selectedId, d => d.Id);
        }

        [RelayCommand]
        private async void Add()
        {
            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddCommissionDecisionViewModel>();
            await vm.PrepareStandaloneAsync();

            var window = new AddCommissionDecisionView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedCommissionDecision == null)
                return;

            using var scope = _scopeFactory.CreateScope();
            var vm = scope.ServiceProvider.GetRequiredService<AddCommissionDecisionViewModel>();
            await vm.InitializeForEditAsync(SelectedCommissionDecision);

            var window = new AddCommissionDecisionView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedCommissionDecision == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedCommissionDecision.DecisionNumber))
                return;

            try
            {
                await _decisionService.DeleteAsync(SelectedCommissionDecision.Id);
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
            if (SelectedCommissionDecision == null)
                return;

            DetailWindowHelper.Show(
                new CommissionDecisionDetailsView(),
                new CommissionDecisionDetailsViewModel(SelectedCommissionDecision));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
