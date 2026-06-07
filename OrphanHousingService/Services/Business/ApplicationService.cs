using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ApplicationService : CrudService<Application>
    {
        public ApplicationService(
            OrphanHousingDbContext context,
            IValidator<Application> validator) : base(context, validator) { }

        public async Task<List<Application>> GetAllAsync()
        {
            return await _context.Applications
                .Include(x => x.Contract)
                    .ThenInclude(c => c.Person)
                .Include(x => x.Contract)
                    .ThenInclude(c => c.Apartment)
                .ToListAsync();
        }

        public async Task CreateAsync(Application application)
        {
            application.Id = Guid.NewGuid();
            application.Comment = NullableStringHelper.Normalize(application.Comment);

            if (string.IsNullOrWhiteSpace(application.ApplicationNumber))
                application.ApplicationNumber = GenerateNumber();

            await AddAsync(application);
        }

        public async Task UpdateAsync(Application application)
        {
            var existing = await GetByIdAsync(application.Id);
            if (existing == null)
                throw new Exception("Заявление не найдено");

            existing.ApplicationDate = application.ApplicationDate;
            existing.Comment = NullableStringHelper.Normalize(application.Comment);
            existing.ApplicationNumber = application.ApplicationNumber;

            await ValidateAsync(existing);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (await _context.CommissionDecisions.AnyAsync(d => d.ApplicationId == id))
                throw new Exception("Невозможно удалить: у заявления есть решения комиссии");

            var app = await GetByIdAsync(id);
            if (app == null)
                throw new Exception("Заявление не найдено");

            await base.RemoveAsync(app);
        }

        public async Task SubmitAsync(Application application)
        {
            application.Id = Guid.NewGuid();
            application.ApplicationDate = DateTime.UtcNow;
            application.Status = ApplicationStatus.Pending;
            application.Comment = NullableStringHelper.Normalize(application.Comment);

            await AddAsync(application);
        }

        public async Task ApproveAsync(Guid id)
        {
            var app = await GetByIdAsync(id);
            if (app == null)
                throw new Exception("Заявление не найдено");

            app.Status = ApplicationStatus.Approved;
            await SaveChangesAsync();
        }

        public async Task RejectAsync(Guid id, string reason)
        {
            var app = await GetByIdAsync(id);
            if (app == null)
                throw new Exception("Заявление не найдено");

            app.Status = ApplicationStatus.Rejected;
            await SaveChangesAsync();
        }
    }
}
