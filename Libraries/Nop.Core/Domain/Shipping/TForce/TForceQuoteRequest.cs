using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping.TForce
{
    public class GetRate
    {
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Service { get; set; }
        public int Pieces { get; set; }
        public int Weight { get; set; }
        public string Accessorials { get; set; }
        public List<Package> Package { get; set; }
    }

    public class Package
    {
        public int length { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int weight { get; set; }
        public string type { get; set; }
    }

    public class TForceQuoteRequest
    {
        public Security security { get; set; }
        public List<GetRate> getRates { get; set; }
    }

    public class Security
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
