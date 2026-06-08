using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrphanHousingService.Services.Business
{
    public class ContractService : CrudService<Contract>
    {
        public ContractService(
            OrphanHousingDbContext context,
            IValidator<Contract> validator) : base(context, validator)
        {
        }

        public async Task<List<Contract>> GetAllAsync()
        {
            var contracts = await _context.Contracts
                .Include(x => x.Person)
                .Include(x => x.Apartment)
                .Include(x => x.PreviousContract)
                .Include(x => x.UtilityDebts)
                .ToListAsync();

            foreach (var c in contracts)
            {
                c.TotalDebt = c.UtilityDebts
                    .Where(d => d.Status != UtilityDebtStatus.Paid)
                    .Sum(d => d.Amount);
            }

            return contracts;
        }

        public async Task<Contract?> GetActiveContractForPersonAsync(Guid personId)
        {
            return await _context.Contracts
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c =>
                    c.PersonId == personId &&
                    c.Status == ContractStatus.Active);
        }

        public async Task<Contract?> GetByIdWithPersonAsync(Guid contractId)
        {
            return await _context.Contracts
                .Include(c => c.Person)
                .FirstOrDefaultAsync(c => c.Id == contractId);
        }

        public async Task CreateAsync(Contract contract)
        {
            contract.Id = Guid.NewGuid();
            contract.Status = ContractStatus.Active;

            if (string.IsNullOrWhiteSpace(contract.ContractNumber))
                contract.ContractNumber = GenerateNumber();

            if (contract is IHasCreatedAt withCreatedAt && withCreatedAt.CreatedAt == default)
                withCreatedAt.CreatedAt = DateTime.UtcNow;

            await ValidateAsync(contract);
            await _context.Contracts.AddAsync(contract);

            _context.ContractHistories.Add(new ContractHistory
            {
                Id = Guid.NewGuid(),
                ContractId = contract.Id,
                ChangeDate = DateTime.UtcNow,
                OperationType = "Создание",
                NewStatus = contract.Status,
                Comment = $"Договор №{contract.ContractNumber}",
                CreatedAt = DateTime.UtcNow
            });

            if (SystemEntityIds.IsCitizen(contract.PersonId))
            {
                var apartment = await _context.Apartments.FindAsync(contract.ApartmentId);
                if (apartment == null)
                    throw new Exception("Квартира не найдена");

                apartment.CurrentStatus = ApartmentStatus.Distributed;
                _context.ApartmentStatusHistories.Add(new ApartmentStatusHistory
                {
                    Id = Guid.NewGuid(),
                    ApartmentId = contract.ApartmentId,
                    Status = ApartmentStatus.Distributed,
                    ChangeDate = DateTime.UtcNow,
                    Basis = $"Оформлен договор №{contract.ContractNumber}",
                    CreatedAt = DateTime.UtcNow
                });
            }

            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Contract contract)
        {
            var existing = await GetByIdAsync(contract.Id);
            if (existing == null)
                throw new Exception("Договор не найден");

            var oldStatus = existing.Status;

            existing.ContractNumber = contract.ContractNumber;
            existing.ContractDate = contract.ContractDate;
            existing.StartDate = contract.StartDate;
            existing.EndDate = contract.EndDate;
            existing.ContractType = contract.ContractType;
            existing.Status = contract.Status;

            await ValidateAsync(existing);
            await SaveChangesAsync();

            if (oldStatus != existing.Status)
            {
                await RecordHistoryAsync(
                    existing.Id,
                    "Изменение статуса",
                    oldStatus,
                    existing.Status);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            if (await _context.UtilityDebts.AnyAsync(d => d.ContractId == id))
                throw new Exception("Невозможно удалить: у договора есть долги");

            if (await _context.FamilyMembers.AnyAsync(f => f.ContractId == id))
                throw new Exception("Невозможно удалить: у договора есть члены семьи");

            if (await _context.Applications.AnyAsync(a => a.ContractId == id))
                throw new Exception("Невозможно удалить: у договора есть заявления");

            var contract = await GetByIdAsync(id);
            if (contract == null)
                throw new Exception("Договор не найден");

            await base.RemoveAsync(contract);
        }

        public async Task CloseAsync(Guid id, string reason)
        {
            var contract = await GetByIdAsync(id);
            if (contract == null)
                throw new Exception("Договор не найден");

            var oldStatus = contract.Status;
            contract.Status = ContractStatus.Expired;

            await SaveChangesAsync();

            await RecordHistoryAsync(
                id,
                "Закрытие",
                oldStatus,
                contract.Status,
                basis: NullableStringHelper.Normalize(reason));
        }

        public async Task<Contract> CreateNewContractFromOld(Guid oldContractId)
        {
            var old = await GetByIdAsync(oldContractId);
            if (old == null)
                throw new Exception("Старый договор не найден");

            old.Status = ContractStatus.Expired;

            var newContract = new Contract
            {
                PersonId = old.PersonId,
                ApartmentId = old.ApartmentId,
                ContractType = old.ContractType,
                ContractDate = old.ContractDate,
                StartDate = old.StartDate,
                EndDate = old.EndDate,
                Status = ContractStatus.Active,
                ContractNumber = $"{old.ContractNumber}-{GenerateNumber()}",
                PreviousContractId = oldContractId
            };

            await CreateAsync(newContract);

            await RecordHistoryAsync(
                oldContractId,
                "Продление (закрытие старого)",
                ContractStatus.Active,
                ContractStatus.Expired);

            return newContract;
        }

        public async Task ExtendAsync(Guid oldContractId)
        {
            var old = await GetByIdAsync(oldContractId);
            if (old == null)
                throw new Exception("Старый договор не найден");

            old.EndDate = old.EndDate.AddYears(5);
            await SaveChangesAsync();

            await RecordHistoryAsync(
                oldContractId,
                "Продление",
                comment: $"Новая дата окончания: {old.EndDate:dd.MM.yyyy}");

            await CreateNewContractFromOld(oldContractId);
        }

        public async Task TransferToSocialRent(Guid oldContractId)
        {
            var old = await GetByIdAsync(oldContractId);
            if (old == null)
                throw new Exception("Старый договор не найден");

            var newContract = await CreateNewContractFromOld(oldContractId);
            old.Status = ContractStatus.Terminated;

            newContract.ContractType = ContractType.SocialRent;
            await SaveChangesAsync();

            await RecordHistoryAsync(
                newContract.Id,
                "Перевод в соц. найм",
                ContractStatus.Active,
                ContractStatus.Active);
        }

        public async Task ShortenContractEndDate(Guid contractId)
        {
            var oldContract = await GetByIdAsync(contractId);
            if (oldContract == null)
                throw new Exception("Договор не найден");

            oldContract.EndDate = oldContract.EndDate.AddYears(-2);
            await SaveChangesAsync();

            await RecordHistoryAsync(
                contractId,
                "Сокращение срока",
                comment: $"Новая дата окончания: {oldContract.EndDate:dd.MM.yyyy}");
        }

        private async Task RecordHistoryAsync(
            Guid contractId,
            string operationType,
            ContractStatus? oldStatus = null,
            ContractStatus? newStatus = null,
            string? basis = null,
            string? comment = null)
        {
            await _context.ContractHistories.AddAsync(new ContractHistory
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
            });
            await SaveChangesAsync();
        }
    }
}
