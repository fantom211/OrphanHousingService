using FluentValidation;
using System;
using System.Linq;
using System.Windows;

namespace OrphanHousingService.ViewModels.Helpers
{
    public static class ValidationDialogHelper
    {
        public static void ShowError(Exception ex)
        {
            var message = ex switch
            {
                ValidationException validationEx when validationEx.Errors?.Any() == true =>
                    string.Join("\n", validationEx.Errors.Select(e => e.ErrorMessage)),
                ValidationException validationEx => validationEx.Message,
                _ => ex.InnerException?.Message ?? ex.Message
            };

            MessageBox.Show(
                message,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
