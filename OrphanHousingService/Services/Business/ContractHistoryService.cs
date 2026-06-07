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
    public class ContractHistoryService : CrudService<ContractHistory>
    {
        public ContractHistoryService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<ContractHistory>> GetByContractIdAsync(Guid contractId)
        {
            return await _context.Set<ContractHistory>()
                .Where(h => h.ContractId == contractId)
                .OrderByDescending(h => h.ChangeDate)
                .ToListAsync();
        }

        public async Task RecordAsync(
            Guid contractId,
            string operationType,
            ContractStatus? oldStatus = null,
            ContractStatus? newStatus = null,
            string? basis = null,
            string? comment = null)
        {
            var history = new ContractHistory
            {
                Id = Guid.NewGuid(),
                ContractId = contractId,
                ChangeDate = DateTime.UtcNow,
                OperationType = operationType,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Basis = NullableStringHelper.Normalize(basis),
                Comment = NullableStringHelper.Normalize(comment),
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<ContractHistory>().AddAsync(history);
            await _context.SaveChangesAsync();
        }
    }
}
