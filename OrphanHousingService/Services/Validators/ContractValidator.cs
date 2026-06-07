using FluentValidation;
using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Validators
{
    public class ContractValidator : AbstractValidator<Contract>
    {
        public ContractValidator()
        {
            RuleFor(x => x.PersonId)
                .NotEmpty()
                .WithMessage("Выберите человека.");

            RuleFor(x => x.ApartmentId)
                .NotEmpty()
                .WithMessage("Выберите квартиру");

            RuleFor(x => x.ContractNumber)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate)
                .WithMessage("Дата начала должна быть раньше даты окончания");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("Введите дату окончания");

            RuleFor(x => x.ContractType)
                .NotEmpty();

            RuleFor(x => x.Status)
                .NotEmpty();
        }
    }
}
