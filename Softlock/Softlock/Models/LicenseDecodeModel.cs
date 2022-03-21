using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Softlock.Models
{
    public class LicenseDecodeModel 
    {
        [Display(Name = "Enter Key to Decrypt")]
        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]", ErrorMessage = "License key can only have 0 - 9 & A - Z Characters.")]
        public string KeyPart1 { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]", ErrorMessage = "License key can only have 0 - 9 & A - Z Characters.")]
        public string KeyPart2 { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]", ErrorMessage = "License key can only have 0 - 9 & A - Z Characters.")]
        public string KeyPart3 { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]", ErrorMessage = "License key can only have 0-9 & A-Z Characters.")]
        public string KeyPart4 { get; set; }

        [Required (ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]",ErrorMessage = "License key can only have 0 - 9 & A - Z Characters.")]
        public string KeyPart5 { get; set; }

        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        //[RegularExpression(@"^[A-Z][0-9]")]
        public string KeyPart6 { get; set; }

        [Display(Name = "Application")]
        public string ApplicationName { get; set; }

        [Display(Name = "Model")]
        public string ModelNumber { get; set; }

        [Display(Name = "Serial Number")]
        public string SerialNumber { get; set; }

        [Display(Name = "Activate By")]
        public DateTime ActivatedBy { get; set; }

        [Display(Name = "Expires")]
        public Decimal ExpiryDays { get; set; }

        [Display(Name = "Options")]
        public string Options { get; set; }

    }
}
