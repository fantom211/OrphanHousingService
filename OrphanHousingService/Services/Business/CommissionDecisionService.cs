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
    public class CommissionDecisionService : CrudService<CommissionDecision>
    {
        public CommissionDecisionService(
            OrphanHousingDbContext context,
            IValidator<CommissionDecision> validator) : base(context, validator) { }

        public async Task<List<CommissionDecision>> GetAllAsync()
        {
            return await _context.CommissionDecisions
                .Include(x => x.Application)
                    .ThenInclude(a => a.Contract)
                .ToListAsync();
        }

        public async Task CreateAsync(CommissionDecision decision)
        {
            decision.Id = Guid.NewGuid();
            decision.DecisionDate = DateTime.UtcNow;
            decision.Reason = NullableStringHelper.Normalize(decision.Reason);
            decision.Comment = NullableStringHelper.Normalize(decision.Comment);

            if (string.IsNullOrWhiteSpace(decision.DecisionNumber))
                decision.DecisionNumber = GenerateNumber();

            await AddAsync(decision);
        }

        public async Task UpdateAsync(CommissionDecision decision)
        {
            var existing = await GetByIdAsync(decision.Id);
            if (existing == null)
                throw new Exception("Решение не найдено");

            existing.DecisionDate = decision.DecisionDate;
            existing.Reason = NullableStringHelper.Normalize(decision.Reason);
            existing.Comment = NullableStringHelper.Normalize(decision.Comment);
            existing.DecisionNumber = decision.DecisionNumber;

            await ValidateAsync(existing);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var decision = await GetByIdAsync(id);
            if (decision == null)
                throw new Exception("Решение не найдено");

            await base.RemoveAsync(decision);
        }

        public async Task<bool> IsApproved(Guid applicationId, DecisionType type)
        {
            return await _context.CommissionDecisions
                .AnyAsync(x =>
                    x.ApplicationId == applicationId &&
                    x.DecisionType == type &&
                    x.Result == DecisionResult.Approved);
        }
    }
}
