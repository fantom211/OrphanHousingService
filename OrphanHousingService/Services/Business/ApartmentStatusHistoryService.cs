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

            var apartment = await _context.Apartments.FindAsync(history.ApartmentId);
            if (apartment != null)
            {
                apartment.CurrentStatus = history.Status;
                await SaveChangesAsync();
            }
        }
    }
}
