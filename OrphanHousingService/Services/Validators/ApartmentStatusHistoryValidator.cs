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
    public class ApartmentStatusHistoryValidator : AbstractValidator<ApartmentStatusHistory>
    {
        public ApartmentStatusHistoryValidator()
        {
            RuleFor(x => x.ApartmentId)
                .NotEmpty()
                .WithMessage("Квартира должна быть указана");

            RuleFor(x => x.ChangeDate)
                .NotEmpty()
                .WithMessage("Дата изменения статуса обязательна")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage("Дата изменения статуса не может быть в будущем");

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Некорректный статус квартиры");

            RuleFor(x => x.Basis)
                .MaximumLength(250)
                .WithMessage("Основание не должно превышать 250 символов");

            RuleFor(x => x.Comment)
                .MaximumLength(500)
                .WithMessage("Комментарий не должен превышать 500 символов");

            RuleFor(x => x)
                .Must(x =>
                    x.Status != default(ApartmentStatus))
                .WithMessage("Статус должен быть задан");
        }
    }
}
