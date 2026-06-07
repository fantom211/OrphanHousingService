using OrphanHousingService.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Models
{
    public class UtilityDebt
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid ContractId { get; set; }

        [ForeignKey(nameof(ContractId))] 
        public Contract Contract { get; set; } = null!; 
        
        [Column(TypeName = "numeric(12,2)")] 
        public decimal Amount { get; set; }

        [Column(TypeName = "date")]
        public DateTime DebtDate { get; set; }

        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public string? Reason { get; set; }
        public UtilityDebtStatus Status { get; set; } = UtilityDebtStatus.Unpaid;

        [Column(TypeName = "date")]
        public DateTime? PaidDate { get; set; }
    }
}
