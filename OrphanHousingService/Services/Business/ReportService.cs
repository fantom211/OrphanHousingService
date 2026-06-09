using Microsoft.EntityFrameworkCore;
using OrphanHousingService.Models;
using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Reports;
using OrphanHousingService.Repository;
using OrphanHousingService.Services.Helpers;

namespace OrphanHousingService.Services.Business
{
    public class ReportService
    {
        private readonly OrphanHousingDbContext _context;

        public ReportService(OrphanHousingDbContext context)
        {
            _context = context;
        }

        public async Task<ReportModel> BuildSummaryReportAsync(
            string? notes = null,
            DateTime? periodFrom = null,
            DateTime? periodTo = null)
        {
            var period = new ReportPeriod { From = periodFrom, To = periodTo };

            var apartmentsQuery = FilterByDate(_context.Apartments, periodFrom, periodTo, a => a.CreatedAt);
            var apartmentHistoriesQuery = FilterByDate(
                _context.ApartmentStatusHistories, periodFrom, periodTo, h => h.ChangeDate);
            var contractsQuery = FilterByDate(_context.Contracts, periodFrom, periodTo, c => c.ContractDate);
            var contractHistoriesQuery = FilterByDate(
                _context.ContractHistories, periodFrom, periodTo, h => h.ChangeDate);

            var totalApartments = await apartmentsQuery.CountAsync();
            var specialFundTransfers = await apartmentHistoriesQuery
                .CountAsync(h => h.Status == ApartmentStatus.InSpecialFund);
            var socialRentTransfers = await apartmentHistoriesQuery
                .CountAsync(h => h.Status == ApartmentStatus.SocialRent);
            var housingFundTransfers = await apartmentHistoriesQuery
                .CountAsync(h => h.Status == ApartmentStatus.Distributed);

            var totalContracts = await contractsQuery.CountAsync();
            var activeContracts = await contractsQuery
                .CountAsync(c => c.Status == ContractStatus.Active);
            var renegotiatedContracts = await contractsQuery
                .CountAsync(c => c.PreviousContractId != null);
            var shortenedContracts = await contractHistoriesQuery
                .CountAsync(h => h.OperationType == "Сокращение срока");
            var extendedContracts = await contractHistoriesQuery
                .CountAsync(h => h.OperationType == "Продление");

            return new ReportModel
            {
                ReportTitle = "Сводный отчёт по жилищному фонду",
                GeneratedAt = DateTime.Now,
                Period = period,
                ApartmentStatistics = new ApartmentStatistics
                {
                    TotalApartments = totalApartments,
                    SpecialHousingFund = specialFundTransfers,
                    SocialHousing = socialRentTransfers,
                    HousingFund = housingFundTransfers
                },
                ContractStatistics = new ContractStatistics
                {
                    TotalContracts = totalContracts,
                    ActiveContracts = activeContracts,
                    RenegotiatedContracts = renegotiatedContracts,
                    ExtendedContracts = extendedContracts,
                    TerminatedContracts = shortenedContracts
                },
                AdditionalInfo = new AdditionalInfo
                {
                    Notes = notes ?? string.Empty
                }
            };
        }

        private static IQueryable<T> FilterByDate<T>(
            IQueryable<T> query,
            DateTime? from,
            DateTime? to,
            System.Linq.Expressions.Expression<Func<T, DateTime>> dateSelector) =>
            ReportQueryHelper.FilterByDate(query, from, to, dateSelector);
    }
}
