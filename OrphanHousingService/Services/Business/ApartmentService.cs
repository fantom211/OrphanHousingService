using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ApartmentService : CrudService<Apartment>
    {
        public ApartmentService(
            OrphanHousingDbContext context,
            IValidator<Apartment> validator) : base(context, validator) { }

        public async Task<List<Apartment>> GetAllAsync()
        {
            return await _context.Apartments.ToListAsync();
        }

        public async Task CreateAsync(Apartment apartment)
        {
            var exists = await _context.Apartments
                .AnyAsync(x => x.Address == apartment.Address);

            if (exists)
                throw new Exception("Квартира уже существует");

            apartment.Id = Guid.NewGuid();
            apartment.CurrentStatus = ApartmentStatus.InSpecialFund;

            if (apartment.InclussionOrderDate.HasValue)
            {
                apartment.InclussionOrderDate =
                    DateTime.SpecifyKind(apartment.InclussionOrderDate.Value, DateTimeKind.Utc);
            }

            if (apartment is IHasCreatedAt withCreatedAt && withCreatedAt.CreatedAt == default)
                withCreatedAt.CreatedAt = DateTime.UtcNow;

            await ValidateAsync(apartment);

            await _context.Apartments.AddAsync(apartment);
            await _context.ApartmentStatusHistories.AddAsync(new ApartmentStatusHistory
            {
                Id = Guid.NewGuid(),
                ApartmentId = apartment.Id,
                Status = ApartmentStatus.InSpecialFund,
                ChangeDate = DateTime.UtcNow,
                Basis = "Первичное включение в спец. жилой фонд",
                CreatedAt = DateTime.UtcNow
            });

            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            var existing = await GetByIdAsync(apartment.Id);
            if (existing == null)
                throw new Exception("Квартира не найдена");

            existing.Address = apartment.Address;
            existing.CadastralNumber = NullableStringHelper.Normalize(apartment.CadastralNumber);
            existing.Area = apartment.Area;
            existing.RoomsCount = apartment.RoomsCount;
            existing.IncludedToFundDate = apartment.IncludedToFundDate;
            existing.InclussionOrderNumber = NullableStringHelper.Normalize(apartment.InclussionOrderNumber);
            existing.InclussionOrderDate = apartment.InclussionOrderDate;

            await ValidateAsync(existing);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            if (await _context.Contracts.AnyAsync(c => c.ApartmentId == id))
                throw new Exception("Невозможно удалить: у квартиры есть договоры");

            var apartment = await GetByIdAsync(id);
            if (apartment == null)
                throw new Exception("Квартира не найдена");

            await base.RemoveAsync(apartment);
        }

        public async Task ChangeStatusAsync(
            Guid id,
            ApartmentStatus status,
            string? reason,
            bool syncApartment = true)
        {
            var apartment = await GetByIdAsync(id);
            if (apartment == null)
                throw new Exception("Квартира не найдена");

            if (syncApartment)
                apartment.CurrentStatus = status;

            apartment.StatusHistory.Add(new ApartmentStatusHistory
            {
                Id = Guid.NewGuid(),
                ApartmentId = id,
                Status = status,
                ChangeDate = DateTime.UtcNow,
                Basis = NullableStringHelper.Normalize(reason),
                CreatedAt = DateTime.UtcNow
            });

            await SaveChangesAsync();
        }

        public async Task TransferToSocialRentAsync(Guid apartmentId, string? reason)
        {
            await ChangeStatusAsync(apartmentId, ApartmentStatus.SocialRent, reason);
        }

        public async Task TransferToSpecialFundAsync(Guid apartmentId, string? reason)
        {
            await ChangeStatusAsync(apartmentId, ApartmentStatus.InSpecialFund, reason);
        }

        public async Task PrivatizeAsync(Guid apartmentId, string? reason)
        {
            await ChangeStatusAsync(apartmentId, ApartmentStatus.Excluded, reason ?? "Приватизация");
        }
    }
}
