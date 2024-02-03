using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public class UPSShippingService : ICustomShippingService
    {
        private readonly ShippingConfig _shippingConfig;

        public UPSShippingService(ShippingConfig shippingConfig)
        {
            _shippingConfig = shippingConfig;
        }

        public void GetShippingQuote()
        {
            throw new NotImplementedException();
        }
    }
}
