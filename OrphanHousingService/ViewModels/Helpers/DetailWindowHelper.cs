using System.Windows;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class DetailWindowHelper
    {
        public static void Show(Window window, object viewModel)
        {
            window.DataContext = viewModel;
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
