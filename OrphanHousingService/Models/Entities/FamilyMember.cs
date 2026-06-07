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
    public class FamilyMember : IHasCreatedAt
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 

        public Guid ContractId { get; set; }

        [ForeignKey(nameof(ContractId))] 
        public Contract Contract { get; set; } = null!; 

        [Required]
        [MaxLength(300)] 
        public string FullName { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }

        public RelationshipType RelationshipType { get; set; }

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public string DisplayRelationshipType =>
            EnumLocalization.GetString(RelationshipType);
    }
}
