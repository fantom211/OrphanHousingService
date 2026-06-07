using FluentValidation;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Дата задолженности не может быть в будущем");

            RuleFor(x => x.PeriodEnd)
            .GreaterThanOrEqualTo(x => x.PeriodStart);

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Причина не должна превышать 500 символов")
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Статус обязателен");

            RuleFor(x => x.PaidDate)
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Дата оплаты не может быть в будущем")
                .When(x => x.PaidDate.HasValue);

            RuleFor(x => x)
                .Must(x => BeValidPaidState(x))
                .WithMessage("Дата оплаты должна быть указана только для оплаченных долгов");
        }

        private bool BeValidPaidState(UtilityDebt debt)
        {
            if (debt.Status == UtilityDebtStatus.Paid)
                return debt.PaidDate.HasValue;

            if (debt.Status == UtilityDebtStatus.Unpaid)
                return !debt.PaidDate.HasValue;

            return true;
        }
    }
}
