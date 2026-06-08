using Microsoft.EntityFrameworkCore;
using FluentValidation;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class UtilityDebtService : CrudService<UtilityDebt>
    {
        public UtilityDebtService(
            OrphanHousingDbContext context,
            IValidator<UtilityDebt> validator) : base(context, validator) { }

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
            await EnsureActiveContractAsync(debt.ContractId);

            debt.Id = Guid.NewGuid();
            debt.Reason = NullableStringHelper.Normalize(debt.Reason);
            NormalizePaidDate(debt);

            await base.AddAsync(debt);
        }

        public async Task UpdateAsync(UtilityDebt debt)
        {
            var existing = await GetByIdAsync(debt.Id);
            if (existing == null)
                throw new Exception("Долг не найден");

            existing.Amount = debt.Amount;
            existing.DebtDate = debt.DebtDate;
            existing.PeriodStart = debt.PeriodStart;
            existing.PeriodEnd = debt.PeriodEnd;
            existing.Reason = NullableStringHelper.Normalize(debt.Reason);
            existing.Status = debt.Status;
            existing.PaidDate = debt.Status == UtilityDebtStatus.Paid ? debt.PaidDate : null;

            NormalizePaidDate(existing);
            await ValidateAsync(existing);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var debt = await GetByIdAsync(id);
            if (debt == null)
                throw new Exception("Долг не найден");

            await base.RemoveAsync(debt);
        }

        public async Task MarkAsPaid(Guid id)
        {
            var debt = await GetByIdAsync(id);
            if (debt == null)
                throw new Exception("Долг не найден");

            debt.Status = UtilityDebtStatus.Paid;
            debt.PaidDate = DateTime.UtcNow;

            await ValidateAsync(debt);
            await SaveChangesAsync();
        }

        private async Task EnsureActiveContractAsync(Guid contractId)
        {
            var contract = await _context.Contracts.FindAsync(contractId);
            if (contract == null)
                throw new Exception("Договор не найден");

            if (contract.Status != ContractStatus.Active)
                throw new Exception("Долг можно создать только для активного договора");
        }

        private static void NormalizePaidDate(UtilityDebt debt)
        {
            if (debt.Status != UtilityDebtStatus.Paid)
                debt.PaidDate = null;
        }
    }
}
