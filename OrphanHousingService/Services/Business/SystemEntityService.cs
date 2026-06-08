using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Repository;

namespace OrphanHousingService.Services.Business
{
    public class SystemEntityService
    {
        private readonly OrphanHousingDbContext _context;

        public SystemEntityService(OrphanHousingDbContext context)
        {
            _context = context;
        }

        public async Task EnsureSeededAsync()
        {
            await EnsureEntityAsync(
                SystemEntityIds.SpecialHousingFund,
                "Специализированный",
                "жилищный фонд",
                string.Empty);

            await EnsureEntityAsync(
                SystemEntityIds.SocialRent,
                "Социальный",
                "найм",
                string.Empty);
        }

        private async Task EnsureEntityAsync(
            Guid id,
            string surName,
            string firstName,
            string lastName)
        {
            var exists = await _context.Persons
                .AnyAsync(p => p.Id == id)
                .ConfigureAwait(false);
            if (exists)
                return;

            _context.Persons.Add(new Person
            {
                Id = id,
                SurName = surName,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
