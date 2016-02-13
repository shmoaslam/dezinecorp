using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Data;
using Nop.Data;
using Nop.Core.Caching;

namespace Nop.Services.Catalog
{
    public partial class AdditionalTierPriceService : IAdditionalTierPriceService
    {
        #region Fields
        private readonly IRepository<AdditionalTierPriceType> _additinalTierPriceTypeRepository;
        #endregion

        #region Ctor
        public AdditionalTierPriceService(IRepository<AdditionalTierPriceType> additinalTierPriceTypeRepository)
        {
            this._additinalTierPriceTypeRepository = additinalTierPriceTypeRepository;
        }
        #endregion

        public AdditionalTierPriceType GetAddtionalPriceType(int typeId)
        {
            if (typeId == 0)
                return null;

            return _additinalTierPriceTypeRepository.GetById(typeId);
        }
    }
}
