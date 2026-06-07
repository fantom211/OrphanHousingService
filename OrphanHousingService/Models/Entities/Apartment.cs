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
    public class Apartment : IHasCreatedAt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Address { get; set; } = null!;

        [MaxLength(100)]
        public string? CadastralNumber { get; set; }

        [Column(TypeName = "numeric(10,2)")]
        public decimal Area { get; set; }

        public int RoomsCount { get; set; }

        public ApartmentStatus CurrentStatus { get; set; }

        [Column(TypeName ="date")]
        public DateTime? IncludedToFundDate { get; set; }

        [MaxLength(100)]
        public string? InclussionOrderNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? InclussionOrderDate { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();

        public ICollection<ApartmentStatusHistory> StatusHistory { get; set; } = new List<ApartmentStatusHistory>();

        [NotMapped]
        public string StatusDisplay =>
            EnumLocalization.GetString(this.CurrentStatus);

    }
}
