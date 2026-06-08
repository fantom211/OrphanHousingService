using FluentValidation;
using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Validators
{
    public class ApplicationValidator : AbstractValidator<Application>
    {
        public ApplicationValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty()
                .WithMessage("Должен быть указан договор");

            RuleFor(x => x.ApplicationType)
                .IsInEnum()
                .WithMessage("Некорректный тип заявления");

            RuleFor(x => x.ApplicationDate)
                .NotEmpty()
                .WithMessage("Дата заявления обязательна")
                .MustNotBeFutureDate("Дата заявления не может быть в будущем");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Некорректный статус заявления");

            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Комментарий не должен превышать 500 символов");


            RuleFor(x => x.ApplicationNumber)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.ApplicationNumber));
        }
    }
}
