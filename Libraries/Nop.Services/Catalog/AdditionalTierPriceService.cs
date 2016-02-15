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
        private readonly IRepository<AdditionalTierPrice> _additionalTierPriceRepository;
        #endregion

        #region Ctor
        public AdditionalTierPriceService(IRepository<AdditionalTierPriceType> additinalTierPriceTypeRepository, IRepository<AdditionalTierPrice> additionalTierPriceRepository)
        {
            this._additinalTierPriceTypeRepository = additinalTierPriceTypeRepository;
            this._additionalTierPriceRepository = additionalTierPriceRepository;
        }
        #endregion

        public IList<AdditionalTierPrice> GetAddtionalPriceType(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;

            return _additionalTierPriceRepository.Table.Where(x=>x.TierPriceId == tierPriceId).ToList();
        }

        public IList<AdditionalTierPriceType> GetAllAdditionalPriceType()
        {
            return _additinalTierPriceTypeRepository.Table.ToList();
        }
    }
}
