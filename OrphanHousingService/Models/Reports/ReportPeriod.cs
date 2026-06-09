namespace OrphanHousingService.Models.Reports
{
    public class ReportPeriod
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }

        public bool IsAllTime => !From.HasValue && !To.HasValue;

        public string DisplayText
        {
            get
            {
                if (IsAllTime)
                    return "За всё время";

                if (From.HasValue && To.HasValue)
                    return $"с {From.Value:dd.MM.yyyy} по {To.Value:dd.MM.yyyy}";

                if (From.HasValue)
                    return $"с {From.Value:dd.MM.yyyy}";

                return $"по {To!.Value:dd.MM.yyyy}";
            }
        }
    }
}
