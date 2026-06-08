using FluentValidation;
using System;

namespace OrphanHousingService.Services.Validators
{
    internal static class DateValidationRules
    {
        public static bool NotInFuture(DateTime date) =>
            date.Date <= DateTime.Today;

        public static IRuleBuilderOptions<T, DateTime> MustNotBeFutureDate<T>(
            this IRuleBuilder<T, DateTime> ruleBuilder,
            string message)
        {
            return ruleBuilder
                .Must(NotInFuture)
                .WithMessage(message);
        }
    }
}
