using FluentValidation;
using OrphanHousingService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Validators
{
    public class FamilyMemberValidator : AbstractValidator<FamilyMember>
    {
        public FamilyMemberValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty()
                .WithMessage("Должен быть указан договор");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("ФИО обязательно")
                .MaximumLength(300)
                .WithMessage("ФИО не должно превышать 300 символов");

            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("Дата рождения обязательна")
                .LessThan(DateTime.Today)
                .WithMessage("Дата рождения не может быть в будущем");

            RuleFor(x => x.RelationshipType)
                .IsInEnum();
        }
    }
}
