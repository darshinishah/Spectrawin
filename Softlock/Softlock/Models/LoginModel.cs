using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Softlock.Models
{
    public class LoginModel
    {
        [Display(Name = "User Name")]
        [Required(ErrorMessage = "*")]
        [EmailAddress (ErrorMessage =" Invalid User Name.")]
        public string UserName { get; set; }

        [Display(Name = "Password")]        
        [Required (ErrorMessage="*")]
        [MaxLength(256)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^\w\s]).{8,}$", ErrorMessage = "Password can not have special characters other than *#@!.")]
        public string Password { get; set; }
    }
}
