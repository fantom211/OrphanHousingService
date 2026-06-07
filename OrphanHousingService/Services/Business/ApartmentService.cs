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
    public class ApartmentService : CrudService<Apartment>
    {
        public ApartmentService(OrphanHousingDbContext context) : base(context) { }

        public async Task CreateAsync(Apartment apartment)
        {
            var exists = await _context.Apartments
                .AnyAsync(x => x.Address == apartment.Address);

            if (exists)
                throw new Exception("Квартира уже существует");

            apartment.Id = Guid.NewGuid();
            apartment.CurrentStatus = ApartmentStatus.InSpecialFund;

            apartment.InclussionOrderDate =
                DateTime.SpecifyKind(apartment.InclussionOrderDate!.Value, DateTimeKind.Utc);

            await AddAsync(apartment);
        }

        //public async Task CreateApartmentAsync(
        //    string address,
        //    string? cadastralNumber,
        //    decimal area,
        //    int roomsCount,
        //    string orderNumber,
        //    DateTime orderDateUtc)
        //{
        //    var exists = await _context.Apartments
        //        .AnyAsync(x => x.Address == address);

        //    if (exists)
        //        throw new Exception("Квартира уже существует");

        //    var apartment = new Apartment
        //    {
        //        Id = Guid.NewGuid(),
        //        Address = address,
        //        CadastralNumber = cadastralNumber,
        //        Area = area,
        //        RoomsCount = roomsCount,
        //        CurrentStatus = ApartmentStatus.InSpecialFund,
        //        InclussionOrderNumber = orderNumber,
        //        InclussionOrderDate = DateTime.SpecifyKind(orderDateUtc, DateTimeKind.Utc).Date
        //    };

        //    apartment.StatusHistory.Add(new ApartmentStatusHistory
        //    {
        //        Id = Guid.NewGuid(),
        //        ApartmentId = apartment.Id,
        //        Status = ApartmentStatus.InSpecialFund,
        //        ChangeDate = DateTime.UtcNow,
        //        Basis = orderNumber
        //    });

        //    await AddAsync(apartment);
        //}

        public async Task ChangeStatusAsync(Guid id, ApartmentStatus status, string reason)
        {
            var apartment = await GetByIdAsync(id);
            if (apartment == null)
                throw new Exception("Квартира не найдена");

            apartment.CurrentStatus = status;

            apartment.StatusHistory.Add(new ApartmentStatusHistory
            {
                Id = Guid.NewGuid(),
                ApartmentId = id,
                Status = status,
                ChangeDate = DateTime.UtcNow,
                Basis = reason
            });

            await SaveChangesAsync();
        }

        public async Task AssignToContractAsync(Guid apartmentId, Guid contractId)
        {
            var apartment = await GetByIdAsync(apartmentId);
            if (apartment == null)
                throw new Exception("Квартира не найдена");

            if (apartment.CurrentStatus != ApartmentStatus.InSpecialFund)
                throw new Exception("Квартира недоступна");

            apartment.CurrentStatus = ApartmentStatus.Distributed;

            apartment.Contracts.Add(new Contract
            {
                Id = Guid.NewGuid(),
                ApartmentId = apartmentId,
                Status = ContractStatus.Active
            });

            await SaveChangesAsync();
        }
    }
}
