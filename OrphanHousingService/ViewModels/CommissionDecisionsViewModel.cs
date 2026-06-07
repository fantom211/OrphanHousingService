using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.ViewModels
{
    public partial class CommissionDecisionsViewModel : ObservableObject, ICrudViewModel
    {
        private readonly CommissionDecisionService _commissionDecisionService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<CommissionDecision> CommissionDecisions { get; } = [];

        [ObservableProperty]
        private CommissionDecision? selectedCommissionDecision;

        public CommissionDecisionsViewModel(
            CommissionDecisionService commissionDecisionService,
            IServiceProvider serviceProvider)
        {
            _commissionDecisionService = commissionDecisionService;
            _serviceProvider = serviceProvider;

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            CommissionDecisions.Clear();

            var items = await _commissionDecisionService.GetAllAsync();

            foreach (var item in items)
                CommissionDecisions.Add(item);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddCommissionDecisionView>();

            if (window.ShowDialog() == true)
            {
                CommissionDecisions.Clear();
                await LoadAsync();
            }
        }

        [RelayCommand]
        private void Edit()
        {
        }

        [RelayCommand]
        private void Delete()
        {
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
