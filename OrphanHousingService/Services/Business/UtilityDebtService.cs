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
    public class UtilityDebtService : CrudService<UtilityDebt>
    {
        public UtilityDebtService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<UtilityDebt>> GetAllAsync()
        {
            return await _context.UtilityDebts
                .Include(x => x.Contract)
                    .ThenInclude(c => c.Person)
                .Include(x => x.Contract)
                    .ThenInclude(c => c.Apartment)
                .ToListAsync();
        }

        public async Task CreateAsync(UtilityDebt debt)
        {
            debt.Id = Guid.NewGuid();

            await base.AddAsync(debt);
        }

        public async Task MarkAsPaid(Guid id)
        {
            var debt = await GetByIdAsync(id);
            if (debt == null)
                throw new Exception("Долг не найден");

            debt.Status = UtilityDebtStatus.Paid;
            debt.PaidDate = DateTime.UtcNow;

            await SaveChangesAsync();
        }
    }
}
