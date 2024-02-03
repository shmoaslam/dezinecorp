using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public class ShippingServiceFactory : IShippingServiceFactory
    {

        public ShippingServiceFactory()
        {

        }
        public ICustomShippingService Create(ShippingConfig company)
        {
            if (company.ShippingCompany == ShippingCompany.TForce)
                return new TForceShippingService(company);
            else if (company.ShippingCompany == ShippingCompany.UPS)
                return new UPSShippingService(company);
            else
                throw new ArgumentException("Invalid type", nameof(company));
        }
    }
}
