using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrphanHousingService.Models
{
    public class ContractHistory : IHasCreatedAt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ContractId { get; set; }

        [ForeignKey(nameof(ContractId))]
        public Contract Contract { get; set; } = null!;

        public DateTime ChangeDate { get; set; }

        [Required]
        [MaxLength(100)]
        public string OperationType { get; set; } = null!;

        public string? Basis { get; set; }

        public string? Comment { get; set; }

        public ContractStatus? OldStatus { get; set; }

        public ContractStatus? NewStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string DisplayOldStatus =>
            OldStatus.HasValue ? EnumLocalization.GetString(OldStatus.Value) : string.Empty;

        [NotMapped]
        public string DisplayNewStatus =>
            NewStatus.HasValue ? EnumLocalization.GetString(NewStatus.Value) : string.Empty;
    }
}
