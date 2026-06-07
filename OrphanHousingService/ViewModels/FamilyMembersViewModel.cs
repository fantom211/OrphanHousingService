using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Business;
using OrphanHousingService.ViewModels.Details;
using OrphanHousingService.ViewModels.Helpers;
using OrphanHousingService.ViewModels.Interfaces;
using OrphanHousingService.Views.CrudViews;
using OrphanHousingService.Views.Details;
using System.Collections.ObjectModel;

namespace OrphanHousingService.ViewModels
{
    public partial class FamilyMembersViewModel : ObservableObject, ICrudViewModel
    {
        private readonly FamilyMemberService _familyMemberService;
        private readonly IServiceProvider _serviceProvider;

        public ObservableCollection<FamilyMember> FamilyMembers { get; } = [];

        [ObservableProperty]
        private FamilyMember? selectedFamilyMember;

        public FamilyMembersViewModel(
            FamilyMemberService familyMemberService,
            IServiceProvider serviceProvider)
        {
            _familyMemberService = familyMemberService;
            _serviceProvider = serviceProvider;
            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            FamilyMembers.Clear();

            var items = await _familyMemberService.GetAllAsync();

            foreach (var item in items)
                FamilyMembers.Add(item);
        }

        [RelayCommand]
        private async void Add()
        {
            var window = _serviceProvider.GetRequiredService<AddFamilyMemberView>();

            window.Owner = System.Windows.Application.Current.MainWindow;

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private void Edit()
        {
        }

        [RelayCommand]
        private void Delete()
        {
        }

        [RelayCommand]
        private void OpenDetails()
        {
            if (SelectedFamilyMember == null)
                return;

            var window = _serviceProvider.GetRequiredService<FamilyMemberDetailsView>();
            DetailWindowHelper.Show(window, new FamilyMemberDetailsViewModel(SelectedFamilyMember));
        }

        IRelayCommand ICrudViewModel.AddCommand => AddCommand;
        IRelayCommand ICrudViewModel.EditCommand => EditCommand;
        IRelayCommand ICrudViewModel.DeleteCommand => DeleteCommand;
    }
}
