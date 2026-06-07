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
    public class ApartmentStatusHistory
    {
        [Key] 
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid ApartmentId { get; set; }

        [ForeignKey(nameof(ApartmentId))] 
        public Apartment Apartment { get; set; } = null!;

        [Column(TypeName = "date")]
        public DateTime ChangeDate { get; set; }
        public ApartmentStatus Status { get; set; }
        public string? Basis { get; set; }
        public string? Comment { get; set; }

        [NotMapped]
        public string DisplayStatus =>
            EnumLocalization.GetString(Status);
    }
}
