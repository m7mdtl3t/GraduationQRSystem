using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationQRSystem.Models
{
    public class Senior
    {
        public int SeniorId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Range(0, 1000)]
        public int NumberOfGuests { get; set; }

        [StringLength(500)]
        public string? QrUrl { get; set; }

        public List<Guest> Guests { get; set; } = new List<Guest>();
    }
}


