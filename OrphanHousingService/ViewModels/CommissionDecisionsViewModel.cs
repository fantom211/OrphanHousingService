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
        private readonly IServiceProvider _serviceProvider;
        private readonly ListCollectionManager<CommissionDecision> _listManager;

        public ICollectionView CommissionDecisions => _listManager.View;

        [ObservableProperty]
        private CommissionDecision? selectedCommissionDecision;

        [ObservableProperty]
        private string? searchText;

        public CommissionDecisionsViewModel(
            CommissionDecisionService decisionService,
            IServiceProvider serviceProvider)
        {
            _decisionService = decisionService;
            _serviceProvider = serviceProvider;
            _listManager = new ListCollectionManager<CommissionDecision>(d => new[]
            {
                d.DecisionNumber,
                d.Application?.ApplicationNumber,
                d.Application?.Contract?.ContractNumber,
                d.Application?.Contract?.Person?.FullName,
                d.Reason,
                d.Comment
            });
            _ = LoadAsync();
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var items = await _decisionService.GetAllAsync();
            _listManager.SetItems(items);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddCommissionDecisionView>();

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Edit()
        {
            if (SelectedCommissionDecision == null)
                return;

            var vm = _serviceProvider.GetRequiredService<AddCommissionDecisionViewModel>();
            vm.InitializeForEdit(SelectedCommissionDecision);

            var window = new AddCommissionDecisionView(vm);

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

            var window = _serviceProvider.GetRequiredService<CommissionDecisionDetailsView>();
            DetailWindowHelper.Show(window, new CommissionDecisionDetailsViewModel(SelectedCommissionDecision));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
