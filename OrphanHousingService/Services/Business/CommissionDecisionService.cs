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
    public class CommissionDecisionService : CrudService<CommissionDecision>
    {
        public CommissionDecisionService(OrphanHousingDbContext context) : base(context) { }

        public async Task<List<CommissionDecision>> GetAllAsync()
        {
            return await _context.CommissionDecisions
                .Include(x => x.Application)
                .ToListAsync();
        }

        //public async Task CreateAsync(CommissionDecision decision)
        //{
        //    decision.Id = Guid.NewGuid();
        //    decision.DecisionDate = DateTime.UtcNow;

        //    await AddAsync(decision);
        //}

        public async Task CreateAsync(CommissionDecision decision)
        {
            decision.Id = Guid.NewGuid();
            decision.DecisionDate = DateTime.UtcNow;
            if (string.IsNullOrWhiteSpace(decision.DecisionNumber))
                decision.DecisionNumber = GenerateNumber();

            await AddAsync(decision);
        }

        public async Task<bool> IsApproved(Guid applicationId, DecisionType type)
        {
            return await _context.CommissionDecisions
                .AnyAsync(x =>
                    x.ApplicationId == applicationId &&
                    x.DecisionType == type &&
                    x.Result == DecisionResult.Approved);
        }

        public async Task ApplyDecisionAsync(CommissionDecision decision)
        {
            await CreateAsync(decision);

            var contract = await _context.Contracts
                .FirstOrDefaultAsync(x => x.Id == decision.ApplicationId);

            if (contract == null)
                throw new Exception("Договор не найден");

            switch (decision.DecisionType)
            {
                case DecisionType.SocialRentTransfer:
                    contract.Status = ContractStatus.Terminated;
                    break;

                case DecisionType.ContractReduction:
                    contract.Status = ContractStatus.Active;
                    break;

                case DecisionType.Extension:
                    contract.Status = ContractStatus.Active;
                    break;

                case DecisionType.ExclusionFromFund:
                    break;
            }

            await _context.SaveChangesAsync();
        }
    }
}
