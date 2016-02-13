using Nop.Core.Domain.Customers;
using System.Collections.Generic;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tier price
    /// </summary>
    public partial class TierPrice : BaseEntity
    {

        private ICollection<AdditionalTierPrice> _additionalTierPrices;

        /// <summary>
        /// Gets or sets the product identifier
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier (0 - all stores)
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer role identifier
        /// </summary>
        public int? CustomerRoleId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the disc
        /// </summary>
        public string Disc { get; set; }

        /// <summary>
        /// Gets or sets the price code
        /// </summary>
        public string PriceCode { get; set; }

        /// <summary>
        /// Gets or sets the product
        /// </summary>
        public virtual Product Product { get; set; }

        /// <summary>
        /// Gets or sets the customer role
        /// </summary>
        public virtual CustomerRole CustomerRole { get; set; }

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
