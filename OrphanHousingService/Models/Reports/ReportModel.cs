namespace OrphanHousingService.Models.Reports
{
    public class ReportModel
    {
        public string ReportTitle { get; set; } = "Отчёт по жилищному фонду";
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public ReportPeriod Period { get; set; } = new();
        public ApartmentStatistics ApartmentStatistics { get; set; } = new();
        public ContractStatistics ContractStatistics { get; set; } = new();
        public AdditionalInfo AdditionalInfo { get; set; } = new();
    }

    public class ApartmentStatistics
    {
        public int TotalApartments { get; set; }
        public int SocialHousing { get; set; }
        public int SpecialHousingFund { get; set; }
        public int HousingFund { get; set; }
    }

    public class ContractStatistics
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int RenegotiatedContracts { get; set; }
        public int ExtendedContracts { get; set; }
        public int TerminatedContracts { get; set; }
    }

    public class AdditionalInfo
    {
        public string Notes { get; set; } = string.Empty;
    }
}
