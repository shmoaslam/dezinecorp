using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Services.Catalog
{

    /// <summary>
    /// Addtional tier price service
    /// </summary>
    public partial interface IAdditionalTierPriceService
    {

        IList<AdditionalTierPrice> GetAddtionalPrice(int tierPriceId);

        IList<AdditionalTierPriceType> GetAllAdditionalPriceType();

        AdditionalTierPriceType GetPriceType(int id);

        void InsertAdditionalTierPrice(AdditionalTierPrice additionalPriceType);
        void UpdateAdditionalTierPrice(AdditionalTierPrice additionalPriceType);
        void DeleteAdditionalTierPrice(int id);
    }
}
