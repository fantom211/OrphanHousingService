using OrphanHousingService.Models.Enums;
using OrphanHousingService.Models.Helpers;
using OrphanHousingService.Services.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Models
{
    public class Application : IHasCreatedAt
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid ContractId { get; set; }

        public string? ApplicationNumber { get; set; }

        [ForeignKey(nameof(ContractId))] 
        public Contract Contract { get; set; } = null!; 
        
        public ApplicationType ApplicationType { get; set; }

        [Column(TypeName = "date")]
        public DateTime ApplicationDate { get; set; }

        [MaxLength(100)]
        public ApplicationStatus Status { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string DisplayApplicationType =>
            EnumLocalization.GetString(this.ApplicationType);

        [NotMapped]
        public string DisplayApplicationStatus =>
           EnumLocalization.GetString(this.Status);

    }
}
