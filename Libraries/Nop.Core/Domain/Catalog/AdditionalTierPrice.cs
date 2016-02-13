using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Catalog
{

    /// <summary>
    /// Represent the addtional price details
    /// </summary>
    public class AdditionalTierPrice : BaseEntity
    {

        /// <summary>
        /// Gets or sets the tier price identifier
        /// </summary>
        public int TierPriceId { get; set; }

        /// <summary>
        /// Gets or sets the additional price type identifier
        /// </summary>
        public int TypeId { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or set the tier price
        /// </summary>
        public TierPrice TierPrice { get; set; }

        /// <summary>
        /// Gets or sets the addtional price type
        /// </summary>
        public AdditionalTierPriceType AdditionalTierPriceType { get; set; }

    }
}
