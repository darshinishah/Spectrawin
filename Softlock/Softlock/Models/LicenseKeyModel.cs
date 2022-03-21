using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Softlock.Models
{
    public class LicenseKeyModel
    {
        [Display (Name = "Enter Key to Decrypt")]
        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart1 { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart2 { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart3 { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart4 { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart5 { get; set; }

        [Required]
        [MaxLength(8, ErrorMessage = "*")]
        [RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart6 { get; set; }        

    }
}
