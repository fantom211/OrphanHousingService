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
    public class ContractService : CrudService<Contract>
    {
        public ContractService(OrphanHousingDbContext context) : base(context) { }

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

        //public async Task CreateAsync(Contract contract)
        //{
        //    contract.Id = Guid.NewGuid();
        //    contract.Status = ContractStatus.Active;
        //    await AddAsync(contract);
        //}

        public async Task CreateAsync(Contract contract)
        {
            contract.Id = Guid.NewGuid();
            contract.Status = ContractStatus.Active;
            if (string.IsNullOrWhiteSpace(contract.ContractNumber))
                contract.ContractNumber = GenerateNumber();

            await AddAsync(contract);
        }

        public async Task CloseAsync(Guid id, string reason)
        {
            var contract = await GetByIdAsync(id);
            if (contract == null)
                throw new Exception("Договор не найден");

            contract.Status = ContractStatus.Expired;

            await SaveChangesAsync();
        }

        public async Task<Contract>CreateNewContractFromOld(Guid oldContractId)
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
            return newContract;
        }

        public async Task ExtendAsync(Guid oldContractId)
        {
            var old = await GetByIdAsync(oldContractId);

            if (old == null)
                throw new Exception("Старый договор не найден");

            old.EndDate = old.EndDate.AddYears(5);
            await SaveChangesAsync();
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
        }

        public async Task ShortenContractEndDate(Guid contractId)
        {
            var oldContract = await GetByIdAsync(contractId);
            if (oldContract == null)
                throw new Exception("Договор не найден");

            oldContract.EndDate = oldContract.EndDate.AddYears(-2);

            await SaveChangesAsync();

            //var newContract = new Contract
            //{
            //    PersonId = oldContract.PersonId,
            //    ApartmentId = oldContract.ApartmentId,
            //    ContractType = oldContract.ContractType,
            //    ContractNumber = oldContract.ContractNumber,
            //    ContractDate = oldContract.ContractDate,
            //    StartDate = oldContract.StartDate,
            //    EndDate = oldContract.EndDate.AddYears(-2),
            //    Status = oldContract.Status
            //};

            //await CreateAsync(oldContract);
        }
    }
}
