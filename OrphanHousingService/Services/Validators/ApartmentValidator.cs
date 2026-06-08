using FluentValidation;
using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Validators
{
    public class ApartmentValidator : AbstractValidator<Apartment>
    {
        public ApartmentValidator()
        {
            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Адрес обязателен")
                .MaximumLength(250)
                .WithMessage("Адрес не должен превышать 250 символов");

            RuleFor(x => x.CadastralNumber)
                .MaximumLength(100)
                .WithMessage("Кадастровый номер не должен превышать 100 символов");

            RuleFor(x => x.Area)
                .GreaterThan(0)
                .WithMessage("Площадь должна быть больше 0");

            RuleFor(x => x.RoomsCount)
                .GreaterThan(0)
                .WithMessage("Количество комнат должно быть больше 0")
                .LessThanOrEqualTo(20)
                .WithMessage("Количество комнат выглядит некорректным");

            RuleFor(x => x.IncludedToFundDate)
                .Must(d => !d.HasValue || DateValidationRules.NotInFuture(d.Value))
                .When(x => x.IncludedToFundDate.HasValue)
                .WithMessage("Дата включения в фонд не может быть в будущем");

            RuleFor(x => x.InclussionOrderNumber)
                .MaximumLength(100)
                .WithMessage("Номер приказа не должен превышать 100 символов");

            RuleFor(x => x.InclussionOrderDate)
                .Must(d => !d.HasValue || DateValidationRules.NotInFuture(d.Value))
                .When(x => x.InclussionOrderDate.HasValue)
                .WithMessage("Дата приказа не может быть в будущем");

            RuleFor(x => x)
                .Must(x =>
                    !x.IncludedToFundDate.HasValue ||
                    !x.InclussionOrderDate.HasValue ||
                    x.InclussionOrderDate <= x.IncludedToFundDate)
                .WithMessage("Дата приказа не может быть позже даты включения в фонд");
        }
    }
}
