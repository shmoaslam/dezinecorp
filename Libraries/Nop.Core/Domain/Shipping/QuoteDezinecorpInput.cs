using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping
{
    public class QuoteDezinecorpInput
    {
        public int Quantity { get; set; }

        public string ProductNumber { get; set; }

        public bool IsResidentialAddress { get; set; }

        public string ZipCode { get; set; }
        public string State { get; set; }  
    }
}
