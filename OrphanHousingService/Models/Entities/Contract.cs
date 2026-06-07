using OrphanHousingService.Models.Enums;
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
    public class Contract
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid PersonId { get; set; }

        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; } = null!;

        public Guid ApartmentId { get; set; }

        [ForeignKey(nameof(ApartmentId))]
        public Apartment Apartment { get; set; } = null!;

        public ContractType ContractType { get; set; }

        [Required]
        [MaxLength(100)]
        public string ContractNumber { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime ContractDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; }

        public Guid? PreviousContractId { get; set; }

        [ForeignKey(nameof(PreviousContractId))]
        public Contract? PreviousContract { get; set; }

        public ICollection<CommissionDecision> Decisions { get; set; } = new List<CommissionDecision>();
        public ICollection<FamilyMember> FamilyMembers { get; set; } = new List<FamilyMember>();
        public ICollection<UtilityDebt> UtilityDebts { get; set; } = new List<UtilityDebt>(); 
        public ICollection<Application> Applications { get; set; } = new List<Application>();

        [NotMapped]
        public string DisplayContractType =>
            EnumLocalization.GetString(this.ContractType);

        [NotMapped]
        public string DisplayStatus =>
            EnumLocalization.GetString(this.Status);

        [NotMapped]
        public decimal TotalDebt { get; set; }
    }
}
