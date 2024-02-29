using Nop.Core.Domain.Common;
using Nop.Core.Domain.Shipping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{
    public partial interface IInfotracDbService
    {
        Task<IEnumerable<QuoteDependentData>> GetDimentionData(string sku, int quantity);

        Task<IEnumerable<InfotracFreightFactor>> GetInfotracFreightFactors();

    }
}
