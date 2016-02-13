using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represent the type of Addtional tier price
    /// </summary>
    public class AdditionalTierPriceType : BaseEntity
    {
        private ICollection<AdditionalTierPrice> _additionalTierPrices;

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the additional prices
        /// </summary>
        public virtual ICollection<AdditionalTierPrice> AdditionalTierPrices
        {
            get { return _additionalTierPrices ?? (_additionalTierPrices = new List<AdditionalTierPrice>()); }
            protected set { _additionalTierPrices = value; }
        }

    }


}
