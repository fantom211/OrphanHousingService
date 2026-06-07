using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class PersonService : CrudService<Person>
    {
        public PersonService(
            OrphanHousingDbContext context,
            IValidator<Person> validator) : base(context, validator) { }

        public async Task<List<Person>> GetAllAsync()
        {
            return await _context.Persons.ToListAsync();
        }

        public async Task CreateAsync(Person person)
        {
            person.PassportData = NullableStringHelper.Normalize(person.PassportData);
            person.Phone = PhoneNormalizer.NormalizeRussianPhone(person.Phone);
            person.Status = NullableStringHelper.Normalize(person.Status);

            if (!string.IsNullOrWhiteSpace(person.PassportData))
            {
                var exists = await _context.Persons
                    .AnyAsync(x => x.PassportData == person.PassportData);

                if (exists)
                    throw new Exception("Человек с такими паспортными данными уже существует");
            }

            person.Id = Guid.NewGuid();
            await AddAsync(person);
        }

        public async Task UpdateAsync(Person person)
        {
            var existing = await GetByIdAsync(person.Id);
            if (existing == null)
                throw new Exception("Человек не найден");

            var passport = NullableStringHelper.Normalize(person.PassportData);
            if (!string.IsNullOrWhiteSpace(passport) && passport != existing.PassportData)
            {
                var exists = await _context.Persons
                    .AnyAsync(x => x.PassportData == passport && x.Id != person.Id);

                if (exists)
                    throw new Exception("Человек с такими паспортными данными уже существует");
            }

            existing.SurName = person.SurName;
            existing.FirstName = person.FirstName;
            existing.LastName = person.LastName;
            existing.BirthDate = person.BirthDate;
            existing.PassportData = passport;
            existing.Phone = PhoneNormalizer.NormalizeRussianPhone(person.Phone);
            existing.Status = NullableStringHelper.Normalize(person.Status);

            await ValidateAsync(existing);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (await _context.Contracts.AnyAsync(c => c.PersonId == id))
                throw new Exception("Невозможно удалить: у человека есть договоры");

            var person = await GetByIdAsync(id);
            if (person == null)
                throw new Exception("Человек не найден");

            await base.RemoveAsync(person);
        }

        public async Task<List<FamilyMember>> GetFamilyMembers(Guid personId)
        {
            var person = await _context.Persons
                .Include(x => x.Contracts)
                    .ThenInclude(c => c.FamilyMembers)
                .FirstOrDefaultAsync(p => p.Id == personId);

            if (person == null)
                return new List<FamilyMember>();

            return person.Contracts
                .SelectMany(c => c.FamilyMembers)
                .ToList();
        }
    }
}
