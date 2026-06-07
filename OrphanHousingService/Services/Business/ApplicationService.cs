using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ApplicationService : CrudService<Application>
    {
        public ApplicationService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<Application>> GetAllAsync()
        {
            return await _context.Applications
                .ToListAsync();
        }

        //public async Task CreateAsync(Application application)
        //{
        //    await _context.AddAsync(application);
        //}

        public async Task CreateAsync(Application application)
        {

                if (string.IsNullOrWhiteSpace(application.ApplicationNumber))
                    application.ApplicationNumber = GenerateNumber();

                await AddAsync(application);
  
        }

        public async Task SubmitAsync(Application application)
        {
            application.Id = Guid.NewGuid();
            application.ApplicationDate = DateTime.UtcNow;
            application.Status = ApplicationStatus.Pending;

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
