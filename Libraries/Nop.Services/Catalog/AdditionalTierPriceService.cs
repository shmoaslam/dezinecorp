using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Data;
using Nop.Data;
using Nop.Core.Caching;
using Nop.Services.Events;
using System.Data;

namespace Nop.Services.Catalog
{
    public partial class AdditionalTierPriceService : IAdditionalTierPriceService
    {
        #region Fields
        private readonly IRepository<AdditionalTierPriceType> _additinalTierPriceTypeRepository;
        private readonly IRepository<AdditionalTierPrice> _additionalTierPriceRepository;
        private readonly IRepository<TierPrice> _tierPrice;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDataProvider _dataProvider;
        private readonly IDbContext _dbContext;
        #endregion

        #region Ctor
        public AdditionalTierPriceService(IRepository<AdditionalTierPriceType> additinalTierPriceTypeRepository
            , IRepository<AdditionalTierPrice> additionalTierPriceRepository
            , IRepository<TierPrice> tierPrice
            , IEventPublisher eventPublisher
            , IDbContext dbContext
            , IDataProvider dataProvider)
        {
            this._additinalTierPriceTypeRepository = additinalTierPriceTypeRepository;
            this._additionalTierPriceRepository = additionalTierPriceRepository;
            this._tierPrice = tierPrice;
            this._eventPublisher = eventPublisher;
            this._dbContext = dbContext;
            this._dataProvider = dataProvider;
        }
        #endregion

        public IList<AdditionalTierPrice> GetAddtionalPrice(int tierPriceId)
        {
            if (tierPriceId == 0)
                return null;

            return _additionalTierPriceRepository.Table.Where(x => x.TierPriceId == tierPriceId).ToList();
        }

        public IList<AdditionalTierPriceType> GetAllAdditionalPriceType(int id = 0)
        {
            if (id == 0) return _additinalTierPriceTypeRepository.Table.ToList();
            else
            {
                var alreadyAddedTierPriceTypeIds = _additionalTierPriceRepository.Table.Where(x=>x.TierPriceId == id).Select(x => x.TypeId).ToList();
                return _additinalTierPriceTypeRepository.Table.Where(x => !alreadyAddedTierPriceTypeIds.Contains(x.Id)).ToList();
            }

        }

        public AdditionalTierPriceType GetPriceType(int id)
        {
            if (id == 0)
                return null;

            return _additinalTierPriceTypeRepository.GetById(id);
        }

        public void InsertAdditionalTierPrice(AdditionalTierPrice additionalPriceType)
        {
            if (additionalPriceType == null)
                throw new ArgumentNullException("additionalPriceType");


            var pPrice = _dataProvider.GetParameter();
            pPrice.ParameterName = "Price";
            pPrice.Value = additionalPriceType.Price;
            pPrice.DbType = DbType.Decimal;

            var pCode = _dataProvider.GetParameter();
            pCode.ParameterName = "Code";
            pCode.Value = additionalPriceType.Code;
            pCode.DbType = DbType.String;

            var pTypeId = _dataProvider.GetParameter();
            pTypeId.ParameterName = "TypeId";
            pTypeId.Value = additionalPriceType.TypeId;
            pTypeId.DbType = DbType.Int32;

            var pTierPriceId = _dataProvider.GetParameter();
            pTierPriceId.ParameterName = "TierPriceId";
            pTierPriceId.Value = additionalPriceType.TierPriceId;
            pTierPriceId.DbType = DbType.Int32;

            _dbContext.ExecuteSqlCommand("insert into AdditionalTierPrice values(@TierPriceId,@TypeId,@Price,@Code)", true, null, pTierPriceId, pTypeId, pPrice, pCode);

            //event notification
            _eventPublisher.EntityInserted(additionalPriceType);
        }

        public void UpdateAdditionalTierPrice(AdditionalTierPrice additionalPriceType)
        {
            if (additionalPriceType == null)
                throw new ArgumentNullException("additionalPriceType");


            var pPrice = _dataProvider.GetParameter();
            pPrice.ParameterName = "Price";
            pPrice.Value = additionalPriceType.Price;
            pPrice.DbType = DbType.Decimal;

            var pCode = _dataProvider.GetParameter();
            pCode.ParameterName = "Code";
            pCode.Value = additionalPriceType.Code;
            pCode.DbType = DbType.String;

            var pTypeId = _dataProvider.GetParameter();
            pTypeId.ParameterName = "TypeId";
            pTypeId.Value = additionalPriceType.TypeId;
            pTypeId.DbType = DbType.Int32;

            var pTierPriceId = _dataProvider.GetParameter();
            pTierPriceId.ParameterName = "TierPriceId";
            pTierPriceId.Value = additionalPriceType.TierPriceId;
            pTierPriceId.DbType = DbType.Int32;

            _dbContext.ExecuteSqlCommand("update AdditionalTierPrice set Price = @Price, Code= @Code, TypeId= @TypeId where TierPriceId = @TierPriceId", true, null, pPrice, pCode, pTypeId, pTierPriceId);

            //event notification
            _eventPublisher.EntityUpdated(additionalPriceType);
        }

        public void DeleteAdditionalTierPrice(int id)
        {
            if (id == 0)
                throw new ArgumentNullException("Parameter not provided!");

            var obj = _additionalTierPriceRepository.GetById(id);

            var pId = _dataProvider.GetParameter();
            pId.ParameterName = "Id";
            pId.Value = id;
            pId.DbType = DbType.Decimal;

            _dbContext.ExecuteSqlCommand("Delete from AdditionalTierPrice where id=@Id", true, null, pId);

            //event notification
            _eventPublisher.EntityDeleted(obj);
        }

        public bool IsAdditionalExists(int tierPriceId, int tierPriceTypeId=0)
        {
            if (tierPriceId == 0)
                throw new ArgumentNullException("Parameter not provided!");

            
            return tierPriceTypeId == 0 ? _additionalTierPriceRepository.Table.Any(x=>x.TierPriceId == tierPriceId)
                :  _additionalTierPriceRepository.Table.Any(x => x.TypeId == tierPriceTypeId && x.TierPriceId == tierPriceId);
        }
    }
}
