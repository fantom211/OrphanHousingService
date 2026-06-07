using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class PersonService : CrudService<Person>
    {
        public PersonService(OrphanHousingDbContext context) : base(context){}

        public async Task CreateAsync(Person person)
        {
            
            var exists = await _context.Persons
                .AnyAsync(x => x.PassportData == person.PassportData);

            if (exists)
                throw new Exception("Человек уже существует");

            person.Id = Guid.NewGuid();
            person.Phone = PhoneNormalizer.NormalizeRussianPhone(person.Phone);
            
            await AddAsync(person);
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
