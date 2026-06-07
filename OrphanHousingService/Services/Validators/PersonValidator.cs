using FluentValidation;
using OrphanHousingService.Models;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Validators
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.SurName)
                .NotEmpty()
                .WithMessage("Фамилия обязательна")
                .MaximumLength(100)
                .WithMessage("Фамилия не должна превышать 100 символов");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Имя обязательно")
                .MaximumLength(100)
                .WithMessage("Имя не должно превышать 100 символов");

            RuleFor(x => x.LastName)
                .MaximumLength(100)
                .WithMessage("Отчество не должно превышать 100 символов")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("Дата рождения обязательна")
                .LessThan(DateTime.Today)
                .WithMessage("Дата рождения не может быть в будущем")
                .Must(BeAValidAge)
                .WithMessage("Возраст выглядит некорректным");

            RuleFor(x => x.PassportData)
                .Matches(@"^\d{4}\s\d{6}$")
                .WithMessage("Паспорт должен быть в формате 'XXXX XXXXXX' (4 цифры, пробел, 6 цифр)")
                .When(x => !string.IsNullOrWhiteSpace(x.PassportData));

            RuleFor(x => x.Phone)
                .Must(PhoneNormalizer.IsValidRussianPhone)
                .WithMessage("Телефон должен быть в формате российского номера (например +7XXXXXXXXXX)")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Status)
                .MaximumLength(100)
                .WithMessage("Статус не должен превышать 100 символов");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.SurName)
                        && !string.IsNullOrWhiteSpace(x.FirstName))
                .WithMessage("ФИО должно быть заполнено");
        }

        private bool BeAValidAge(DateTime birthDate)
        {
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return age is >= 0 and <= 120;
        }
    }
}
