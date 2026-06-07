using FluentValidation;
using System;
using System.Windows;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class ValidationDialogHelper
    {
        public static void ShowError(Exception ex)
        {
            var message = ex switch
            {
                ValidationException validationEx => validationEx.Message,
                _ => ex.Message
            };

            MessageBox.Show(
                message,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
