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
    public class ApartmentStatusHistoryService : CrudService<ApartmentStatusHistory>
    {
        public ApartmentStatusHistoryService(
            OrphanHousingDbContext context,
            IValidator<ApartmentStatusHistory> validator) : base(context, validator) { }

        public async Task<List<ApartmentStatusHistory>> GetAllAsync()
        {
            return await _context.ApartmentStatusHistories
                .AsNoTracking()
                .Include(h => h.Apartment)
                .ToListAsync();
        }

        public async Task CreateAsync(ApartmentStatusHistory history)
        {
            history.Id = Guid.NewGuid();
            history.ChangeDate = history.ChangeDate == default
                ? DateTime.UtcNow
                : history.ChangeDate;
            history.Basis = NullableStringHelper.Normalize(history.Basis);
            history.Comment = NullableStringHelper.Normalize(history.Comment);

            await base.AddAsync(history);

            await _context.Apartments
                .Where(a => a.Id == history.ApartmentId)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(a => a.CurrentStatus, history.Status));
        }
    }
}
