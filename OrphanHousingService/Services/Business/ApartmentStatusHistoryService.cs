using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ApartmentStatusHistoryService : CrudService<ApartmentStatusHistory>
    {
        public ApartmentStatusHistoryService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<ApartmentStatusHistory>> GetAllAsync()
        {
            return await _context.ApartmentStatusHistories
                .Include(h => h.Apartment)
                .ToListAsync();
        }

        public async Task CreateAsync(ApartmentStatusHistory history)
        {
            history.Id = Guid.NewGuid();
            history.ChangeDate = DateTime.UtcNow;

            await base.AddAsync(history);
        }
    }
}
