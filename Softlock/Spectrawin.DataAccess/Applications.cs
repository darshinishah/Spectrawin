using System;

namespace Spectrawin.DataAccess
{
    public class Applications
    {        
        public int ID { get; set; }

        public string ApplicationName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public string AppValues { get; set; }
    }
}
