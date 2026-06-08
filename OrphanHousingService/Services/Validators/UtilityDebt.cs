using FluentValidation;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;

namespace OrphanHousingService.Services.Validators
{
    public class UtilityDebtValidator : AbstractValidator<UtilityDebt>
    {
        public UtilityDebtValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty()
                .WithMessage("Договор обязателен");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Сумма задолженности должна быть больше 0");

            RuleFor(x => x.DebtDate)
                .NotEmpty()
                .WithMessage("Дата задолженности обязательна")
                .MustNotBeFutureDate("Дата задолженности не может быть в будущем");

            RuleFor(x => x.PeriodEnd)
                .GreaterThanOrEqualTo(x => x.PeriodStart)
                .WithMessage("Конец периода не может быть раньше начала");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Причина не должна превышать 500 символов")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Статус обязателен");

            RuleFor(x => x.PaidDate)
                .Must(d => !d.HasValue || DateValidationRules.NotInFuture(d.Value))
                .WithMessage("Дата оплаты не может быть в будущем")
                .When(x => x.PaidDate.HasValue);

            RuleFor(x => x)
                .Must(BeValidPaidState)
                .WithMessage("Для оплаченного долга укажите дату оплаты");
        }

        private static bool BeValidPaidState(UtilityDebt debt)
        {
            if (debt.Status == UtilityDebtStatus.Paid)
                return debt.PaidDate.HasValue;

            if (debt.Status == UtilityDebtStatus.Unpaid)
                return !debt.PaidDate.HasValue;

            return true;
        }
    }
}
