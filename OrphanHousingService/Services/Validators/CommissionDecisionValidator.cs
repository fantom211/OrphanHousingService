using FluentValidation;
using OrphanHousingService.Models;

namespace OrphanHousingService.Services.Validators
{
    public class CommissionDecisionValidator : AbstractValidator<CommissionDecision>
    {
        public CommissionDecisionValidator()
        {
            RuleFor(x => x.ApplicationId)
                .NotEmpty()
                .WithMessage("Заявление должно быть указано");

            RuleFor(x => x.DecisionType)
                .IsInEnum()
                .WithMessage("Некорректный тип решения комиссии");

            RuleFor(x => x.DecisionNumber)
                .NotEmpty()
                .WithMessage("Номер решения обязателен")
                .MaximumLength(100)
                .WithMessage("Номер решения не должен превышать 100 символов");

            RuleFor(x => x.DecisionDate)
                .NotEmpty()
                .WithMessage("Дата решения обязательна")
                .MustNotBeFutureDate("Дата решения не может быть в будущем");

            RuleFor(x => x.Result)
                .IsInEnum()
                .WithMessage("Некорректный результат решения");

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage("Причина не должна превышать 500 символов");

            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Комментарий не должен превышать 500 символов");
        }
    }
}
