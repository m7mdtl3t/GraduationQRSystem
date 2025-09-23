using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GraduationQRSystem.Models
{
    public class Guest
    {
        public int GuestId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "رقم الموبايل لازم يكون مصري صحيح")]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsAttended { get; set; }

        public DateTime? AttendanceTime { get; set; }

        [ForeignKey("Senior")]
        public int SeniorId { get; set; }
        public Senior? Senior { get; set; }
    }
}


