using System;
using System.Collections.Generic;
using System.Text;

namespace Spectrawin.DataAccess
{
    public class Users
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailId { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public string UserRole { get; set; }
    }
}
