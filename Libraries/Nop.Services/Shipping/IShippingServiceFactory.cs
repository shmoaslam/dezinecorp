using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Shipping
{
    public interface IShippingServiceFactory
    {
        ICustomShippingService Create(ShippingConfig company);
    }
}
