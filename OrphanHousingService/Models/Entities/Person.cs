using OrphanHousingService.Models.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrphanHousingService.Models
{
    public class Person : IHasCreatedAt
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string SurName { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [MaxLength(100)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }

        [MaxLength(200)]
        public string? PassportData { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();


        [NotMapped]
        public string FullName =>
            string.Join(" ",
                new[] { SurName, FirstName, LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
