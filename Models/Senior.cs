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

        [Required]
        [Phone]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "رقم الموبايل لازم يكون مصري صحيح")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string? QrUrl { get; set; }

        public List<Guest> Guests { get; set; } = new List<Guest>();
    }
}


