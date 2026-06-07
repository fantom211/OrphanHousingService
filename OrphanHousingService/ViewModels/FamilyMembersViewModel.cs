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
    public partial class FamilyMembersViewModel : ObservableObject, ISearchableListViewModel
    {
        private readonly FamilyMemberService _familyMemberService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ListCollectionManager<FamilyMember> _listManager;

        public ICollectionView FamilyMembers => _listManager.View;

        [ObservableProperty]
        private FamilyMember? selectedFamilyMember;

        [ObservableProperty]
        private string? searchText;

        public FamilyMembersViewModel(
            FamilyMemberService familyMemberService,
            IServiceProvider serviceProvider)
        {
            _familyMemberService = familyMemberService;
            _serviceProvider = serviceProvider;
            _listManager = new ListCollectionManager<FamilyMember>(m => new[]
            {
                m.Contract?.ContractNumber,
                m.Contract?.Person?.FullName,
                m.FullName
            });
            _ = LoadAsync();
        }

        partial void OnSearchTextChanged(string? value) => _listManager.SearchText = value;

        public async Task LoadAsync()
        {
            var items = await _familyMemberService.GetAllAsync();
            _listManager.SetItems(items);
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
        private async void Edit()
        {
            if (SelectedFamilyMember == null)
                return;

            var vm = _serviceProvider.GetRequiredService<AddFamilyMemberViewModel>();
            vm.InitializeForEdit(SelectedFamilyMember);

            var window = new AddFamilyMemberView(vm)
            {
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (window.ShowDialog() == true)
                await LoadAsync();
        }

        [RelayCommand]
        private async void Delete()
        {
            if (SelectedFamilyMember == null)
                return;

            if (!CrudDialogHelper.ConfirmDelete(SelectedFamilyMember.FullName))
                return;

            try
            {
                await _familyMemberService.RemoveAsync(SelectedFamilyMember.Id);
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
