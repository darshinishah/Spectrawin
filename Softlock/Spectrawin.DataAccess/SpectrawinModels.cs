using System;

namespace Spectrawin.DataAccess
{
    public class SpectrawinModels
    {        
        public int ID { get; set; }

        public int ModelNumber { get; set; }

        public string ModelName { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        
    }
}
