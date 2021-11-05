using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactSalesOrder.Models
{
    // This class Models the data that is read from the database into C# Objects
    public class Customer
    {
        public string Name { get; set; }
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string ModifiedDate { get; set; }
    }
}
