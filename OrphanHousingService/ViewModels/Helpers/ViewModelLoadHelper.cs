using System;
using System.Threading.Tasks;
using System.Windows;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class ViewModelLoadHelper
    {
        public static async Task RunSafeAsync(Func<Task> load, string context)
        {
            try
            {
                await load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось загрузить данные ({context}):\n{ex.Message}",
                    "Ошибка загрузки",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
