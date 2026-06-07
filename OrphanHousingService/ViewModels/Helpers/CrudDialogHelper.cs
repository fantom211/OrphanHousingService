using System.Windows;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class CrudDialogHelper
    {
        public static bool ConfirmDelete(string entityName)
        {
            var result = MessageBox.Show(
                $"Удалить «{entityName}»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            return result == MessageBoxResult.Yes;
        }
    }
}
