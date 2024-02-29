using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Shipping
{
    public class ShippingConfig
    {
        public ShippingCompany ShippingCompany { get; set; }

        public string AuthUrl { get; set; }

        public string QuoteUrl { get; set; }

        public string BasePath { get; set; }
        public string AuthJsonFile { get; set; }

        public string QuoteJsonFile { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
