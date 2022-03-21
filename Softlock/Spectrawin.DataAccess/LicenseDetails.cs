using System;
using System.Collections.Generic;
using System.Text;

namespace Spectrawin.DataAccess
{
    public class LicenseDetails
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string Application { get; set; }
        public int ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Key { get; set; }
        public int ExpirationDays { get; set; }
        public string LicenseOptions { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LicenseType { get; set; }
    }
}
