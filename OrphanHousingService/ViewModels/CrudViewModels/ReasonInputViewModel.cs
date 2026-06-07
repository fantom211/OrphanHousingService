using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace OrphanHousingService.ViewModels.CrudViewModels
{
    public partial class ReasonInputViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title = "Укажите причину";

        [ObservableProperty]
        private string prompt = "Причина / основание:";

        [ObservableProperty]
        private string? reason;

        public Action<bool>? CloseAction { get; set; }

        [RelayCommand]
        private void Confirm()
        {
            CloseAction?.Invoke(true);
        }

        [RelayCommand]
        private void Cancel()
        {
            CloseAction?.Invoke(false);
        }
    }
}
