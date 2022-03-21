using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Softlock.Models
{
    public class LicenseDetailModel
    {
        [Display(Name = "Application", Prompt = "Please select Application", Description = "Application")]
        [Required(ErrorMessage = "*")]
        public string Application { get; set; }

        //[Display(Name = "Order Type", Prompt = "Please select Order Type", Description = "Order Type")]
        //public string OrderType { get; set; }

        [Display(Name = "Model Number", Prompt = "Please select Model Number", Description = "Model Number")]
        [Required(ErrorMessage = "*")]
        public int ModelNumber { get; set; }

        [Display(Name = "Serial Number", Prompt = "Please enter Serial Number", Description = "Serial Number")]
        [Required(ErrorMessage = "*")]
        [MaxLength(8)]
        public string SerialNumber { get; set; }

        [Display(Name = "Expiration Days", Prompt = "Please enter Expiration Days", Description = "Expiration Days")]        
        [Required(ErrorMessage = "*")]
        public int ExpirationDays { get; set; }

        [Display(Name = "Customer Email", Prompt = "Please enter Customer Email", Description = "Customer Email")]        
        [EmailAddress(ErrorMessage = " Invalid Customer Email.")]
        public string CustomerEmail { get; set; }

        [Display(Name = "Order Number", Prompt = "Please enter Application", Description = "Order Number")]
        [Required(ErrorMessage = "*")]
        public string OrderNumber { get; set; }

        [Display(Name = "Customer Name", Prompt = "Please enter Customer Name", Description = "Customer Name")]
        [Required(ErrorMessage = "*")]
        public string CustomerName { get; set; }

        [Display(Name = "License Options", Prompt = "Please select License Options", Description = "License Options")]
        [Required(ErrorMessage = "*")]
        public string[] LicenseOptions { get; set; }

        public List<KeyValuePair<string, string>> ApplicationOptions { get; set; }

        public string LicenseType { get; set; }
        public long Options { get; set; }
        
    }
}
