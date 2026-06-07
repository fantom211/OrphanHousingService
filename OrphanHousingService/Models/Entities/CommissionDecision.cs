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
    public class CommissionDecision : IHasCreatedAt
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid ApplicationId { get; set; }

        [ForeignKey(nameof(ApplicationId))] 
        public Application Application { get; set; } = null!;
        public DecisionType DecisionType { get; set; }

        [Required]
        [MaxLength(100)] 
        public string DecisionNumber { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime DecisionDate { get; set; }
        public DecisionResult Result { get; set; }
        public string? Reason { get; set; }
        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string DisplayDecisionType =>
           EnumLocalization.GetString(this.DecisionType);

        [NotMapped]
        public string DisplayResult =>
           EnumLocalization.GetString(this.Result);
    }
}
